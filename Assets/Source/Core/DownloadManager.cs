using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Collections;

namespace Core
{
	public abstract class DynamicFile
	{
		public event Action<DynamicFile> Updated;
		
		private bool _loaded;
		public bool IsLoaded
		{
			get
			{
				return _loaded;
			}
			set
			{
				_loaded = value;
								
				if(Updated != null)
					Updated(this);					
			}
		}
		
		public string FileName;
		public string Path;
		
		public abstract void Load(WWW www);
	}
	
	public abstract class DynamicImage:DynamicFile
	{		
		public bool IsSupported;
		
		public Texture2D Texture
		{
			get;
			private set;
		}
		
		public override void Load (WWW www)
		{
			this.Texture = www.texture;
			
			if(this.Texture == null)
				IsSupported = false;
			else  if(www.bytes.Length >= 3)
			{
				IsSupported = !(www.bytes[0] == 'G' && www.bytes[1] == 'I' && www.bytes[2] == 'F');
			}
		}
	}	
	
	public sealed class DownloadManagerWorkState:StateCommand
	{
		protected override void OnStart (object[] args)
		{
			
		}
	}
	
	public abstract class DownloadManagerCommand:StateCommand
	{
		private DynamicFile _file;
		private WWW _www;
		
		public DynamicFile File
		{
			get
			{
				return _file;
			}
		}			
		
		protected sealed override void OnStart (object[] args)
		{
			_file = (DynamicFile)args[0];
			
			if(System.IO.File.Exists(_file.Path))
			{				
				StartCoroutine(LoadFile(_file.Path));
			}	
			else
			{
				Download();					
			}
			
		}
		
		protected IEnumerator LoadFile(string path)
		{
			return LoadURL(string.Format("file://{0}", _file.Path));
		}
			
		protected IEnumerator LoadURL(string url) 
		{				
			_www = new WWW(url);
			
	        yield return _www;
			
			if(String.IsNullOrEmpty( _www.error ))
			{
				_file.Load(_www);
				_file.IsLoaded = true;
				
				if(!System.IO.File.Exists(_file.Path))
				{
					try
					{
						using(FileStream stream = System.IO.File.OpenWrite(_file.Path))
						{
							stream.Write(_www.bytes,0,_www.bytes.Length);
						}
					}
					catch(Exception e)
					{
						MonoLog.Log(MonoLogChannel.Core, "Unable to store file to path " + _file.Path, e);					
					}
				}
				
				FinishCommand();
			}			
			else
			{
				MonoLog.Log(MonoLogChannel.Core, "Unable to load url " + url + ". Error:" + _www.error);
				
				_file.IsLoaded = false;
				
				FinishCommand(false);				
			}
    	}
		
		protected override void OnReleaseResources ()
		{
			if(_www != null)
			{
				_www.Dispose();
				_www = null;
			}
		}
		
		protected abstract void Download();
	}
	
	public sealed class DownloadManager:MonoSingleton<DownloadManager>,IStateMachineContainer
	{
		private const int MAX_DOWNLOAD_FILE = 10;
		
		private readonly StateMachine _stateMachine;
		private readonly Dictionary<Type,Type> _fileToCommand;
		
		public static event Action<DynamicFile> Complete;
		public static event Action<DynamicFile> Fault;
		
		public const string FOLDER = "DownloadCache";
		
		private string _folderPath;
		private List<string> _downloadQueue;
		private Dictionary<string,DynamicFile> _cache;
		

		
		public DownloadManager ()
		{
			_fileToCommand = new Dictionary<Type, Type>();
			_stateMachine = new StateMachine(this);
			_downloadQueue = new List<string>();
			_cache = new Dictionary<string, DynamicFile>();
		}
		
		public void RegisterType<TFile,TCommand>()
			where TFile:DynamicFile
			where TCommand:DownloadManagerCommand
				
		{
			_fileToCommand.Add(typeof(TFile),typeof(TCommand));
		}
		
		protected override void Init ()
		{
			_folderPath = Path.Combine(Application.persistentDataPath,FOLDER);
			
			if(!Directory.Exists(_folderPath))
				Directory.CreateDirectory(_folderPath);			
		}
		
		public DynamicFile Download(DynamicFile file)
		{						
			file.Path = Path.Combine(_folderPath,file.FileName);
				
			if(_cache.ContainsKey(file.Path))
			{
				return _cache[file.Path];
			}
			else
			{		
				_downloadQueue.Add(file.Path);
				_cache.Add(file.Path,file);
				
				_stateMachine.Execute(_fileToCommand[file.GetType()],file).AsyncToken.AddResponder(
						new Responder<StateCommand>(OnDownloadFinished,OnDownloadFault));				
			}
						
			return file;
		}
		
		private void OnDownloadFinished(StateCommand command)
		{
			DownloadManagerCommand downloadManagerCommand = (DownloadManagerCommand)command;
						
			_downloadQueue.Remove(downloadManagerCommand.File.Path);
			
			if(Complete != null)
				Complete(downloadManagerCommand.File);
		}
		
		private void OnDownloadFault(StateCommand command)
		{
			DownloadManagerCommand downloadManagerCommand = (DownloadManagerCommand)command;
			
			_downloadQueue.Remove(downloadManagerCommand.File.Path);
			
			if(Fault != null)
				Fault(downloadManagerCommand.File);
		}
		
		#region IStateMachineContainer implementation
		public void Next (StateCommand previousState)
		{
			
		}

		public UnityEngine.GameObject GameObject 
		{
			get 
			{
				return this.gameObject;
			}
		}
		#endregion
	}
}

