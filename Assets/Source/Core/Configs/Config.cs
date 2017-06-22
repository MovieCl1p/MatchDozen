using System;
using UnityEngine;
using System.IO;
using Core;
using Data.Serialization;

namespace Configs
{
	public abstract class Config<TConfig, TSerializationPolicy> : Observable
		where TSerializationPolicy : SerializationPolicy, new()
		where TConfig : Config<TConfig, TSerializationPolicy>
	{
		public const string CONFIGS_FOLDER = "Configs";
		
		private String _name;
        
        public Config (String name)
		{
			_name = name;
		}
		
		public virtual void OnRestored()
		{
			
		}
		
		public static TConfig Load(String name) 
		{
			TSerializationPolicy serializer = new TSerializationPolicy();
			
			string filepath = Path.Combine(Path.Combine(Application.persistentDataPath,CONFIGS_FOLDER), name);
			
			#if UNITY_EDITOR
				filepath = Path.Combine(Path.Combine(Path.Combine(Application.dataPath,"Development"), CONFIGS_FOLDER), name);
			#endif
			
			TConfig config = null;
			
			using(Stream stream = File.OpenRead(filepath))
			{
				config = serializer.Restore<TConfig>(stream);
			}
			
			config.OnRestored();
			
			return config;
		}
		
		public static TConfig LoadFromResources(String name) 
		{
			TSerializationPolicy serializer = new TSerializationPolicy();
			
			string path = Path.Combine(CONFIGS_FOLDER, name);
			path = path.Replace(Path.GetExtension(path), "");
			
			MonoLog.Log(MonoLogChannel.Core, "LoadFromResources: " +  path);
			
			UnityEngine.Object textAsset = Resources.Load(path);
			
			MonoLog.Log(MonoLogChannel.Core, "textAssets: " + textAsset);
			MonoLog.Log(MonoLogChannel.Core, "textAssets: " + textAsset.GetType());
			
			TConfig config = serializer.RestoreFromString<TConfig>(((TextAsset)textAsset).text);
			
			config.OnRestored();
			
			return config;
		}
        
		public void Save(string name)
		{
			String configFolderPath = Path.Combine(Application.persistentDataPath,CONFIGS_FOLDER);
			
			#if UNITY_EDITOR
				configFolderPath = Path.Combine(Path.Combine(Application.dataPath,"Development"), CONFIGS_FOLDER);
			#endif
			
			if(!Directory.Exists(configFolderPath))
				Directory.CreateDirectory(configFolderPath);
			
			TSerializationPolicy serializer = new TSerializationPolicy();
			
			String configFilePath = Path.Combine(configFolderPath, name);
			
			MonoLog.Log(MonoLogChannel.Configs,"Saving config " + configFilePath);
			
			using(Stream stream = File.Open(Path.Combine(configFolderPath, name), FileMode.Create,FileAccess.Write))
			{
				 serializer.Store<TConfig>((TConfig)this,stream);
			}
		}
		
		public void Save()
		{
			Save(_name);
		}
	}
}

