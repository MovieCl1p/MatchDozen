using System;
using UI;
using UnityEngine;

namespace Core
{
	public sealed class SingletoneAttribute:Attribute
	{
		
	}
	
	public class MonoCommandScopeAttribute:Attribute
	{
		public readonly bool IsApplication;
		
		public MonoCommandScopeAttribute(bool application)
		{
			IsApplication = application; 
		}
	}
	
	public class MonoCommandScopeApplicationAttribute:MonoCommandScopeAttribute
	{
		public MonoCommandScopeApplicationAttribute():base(true)
		{
			
		}
	}
		
	public class MonoCommandScopeSceneAttribute:MonoCommandScopeAttribute
	{
		public MonoCommandScopeSceneAttribute():base(false)
		{
			
		}
	}	
	
	public interface ICommand
	{
		 void OnStart(object[] args);
	}
	
	public enum CommandScope
	{
		Application,
		Scene,
		GameObject
	}
	
	public abstract class MonoCommandTriggerAttribute:Attribute
	{
		public virtual void OnCommandStart(MonoCommand command, object[] args)
		{
			
		}
	}
	
	public abstract class MonoCommand:MonoScheduledBehaviour
	{
		private object[] _args;
		
		internal CommandScope Scope;
		
		bool _running;
		bool _started;
		bool _resourcesReleased;
			
		private bool _success;
		private bool _hold;
		
		public bool IsHold
		{
			get
			{
				return _hold;
			}
			
			set
			{
				_hold  = value;
				
				OnHold();
			}
		}
		
		protected virtual void OnHold()
		{
			
		}
		
		public bool FinishedWithSuccess
		{
			get
			{
				return _success;
			}
		}
		
		public bool IsRunning
		{
			get
			{
				return _running && _started;
			}
		}

		public MonoCommand ()
		{
			_running = true;			
			
			MonoLog.Log(MonoLogChannel.Core, String.Format("Created command {0}",this.GetType().Name));			
		}
		
		protected override sealed void Start()
		{
			if(_running)
			{
				_started = true;
			
				MonoLog.Log(MonoLogChannel.Core, String.Format("Executing command {0}",this.GetType().Name));
							
				try
				{
					OnStart(_args);
				}
				catch(Exception e)
				{
					MonoLog.Log(MonoLogChannel.Core,String.Format("Unable to execute command {0}", this.GetType().Name),e);
					
					FinishCommand(false);
					return;
				}
				
				object[] attributes = this.GetType().GetCustomAttributes(true);
				
				foreach(object attribute in attributes)
				{
					if(attribute is MonoCommandTriggerAttribute)
					{
						MonoCommandTriggerAttribute trigger = (MonoCommandTriggerAttribute)attribute;	
						
						try
						{
							trigger.OnCommandStart(this, _args);	
						}
						catch(Exception e)
						{
							MonoLog.Log(MonoLogChannel.Core,String.Format("Unable to execute trigger {0}", trigger.GetType().Name),e);
						}
					}
				}				
			}
		}
	
		
		protected abstract void OnStart(object[] args);
		
		public static TCommand Execute<TCommand>()
			where TCommand:MonoCommand,new()
		{
			return Execute<TCommand>(new object[]{});
		}
		
		
		protected sealed override void Update()
		{
			if(!_hold)
			{				
				if(_running)
				{
					OnUpdate();
				}
				
				if(_running)
				{
					base.Update();				
				}
			}
		}
		
		
		protected virtual void OnUpdate()
		{
			
		}
		
		public static TCommand ExecuteOn<TCommand>(GameObject target, object[] args)
			where TCommand:MonoCommand,new()

		{
			TCommand result = target.AddComponent<TCommand>();
			
			result._args = args;
			result.Scope = CommandScope.GameObject;
			
			return result;
		}
		
		public static MonoCommand ExecuteOn(Type type, GameObject target, object[] args)
		{
			MonoCommand result = (MonoCommand)target.AddComponent(type);
			
			result._args = args;
			result.Scope = CommandScope.GameObject;
			
			return result;
		}
		
		public static TCommand Execute<TCommand>(params object[] args)
			where TCommand:MonoCommand,new()
		{
			GameObject mgGameObject = MonoSingleton.GetMGGameObject();
			
			object[] attibutes = typeof(TCommand).GetCustomAttributes(true);
			
			bool oneItemOnScene = false;
			CommandScope scope = CommandScope.Application;
			
			foreach(Attribute eachAttribute in attibutes)
			{
				if(eachAttribute is SingletoneAttribute)
				{
					oneItemOnScene = true;
				}
				else if(eachAttribute is MonoCommandScopeAttribute)
				{
					MonoCommandScopeAttribute monoCommandScopeAttribute = (MonoCommandScopeAttribute)eachAttribute;
					
					scope = monoCommandScopeAttribute.IsApplication ? CommandScope.Application : CommandScope.Scene;
				}
			}
			
			if(oneItemOnScene)
			{
				TCommand command = (TCommand)UnityEngine.Object.FindObjectOfType(typeof(TCommand));
				
				if(command != null)
				{
					MonoLog.Log(MonoLogChannel.Core,"Found existing command " + command);
					
					return command;
				}
			}
			
			GameObject target = null;
						
			if( scope == CommandScope.Application )
			{
				target = new GameObject(typeof(TCommand).Name);
				target.transform.parent = mgGameObject.transform;

				UnityEngine.Object.DontDestroyOnLoad(target);				
			}
			else
				target = UIManager.CurrentSceneController.gameObject;
			
			TCommand result = ExecuteOn<TCommand>(target,args);
			
			result.Scope = scope;
									
			return result;
		}
				
		protected virtual void OnFinishCommand(bool success = true){}
		
	 	protected void FinishCommand(bool success = true)
		{
			if(_running)
			{
				_running = false;
				_success = success;
				
				MonoLog.Log(MonoLogChannel.Core, "Finishing command " + this.GetType().Name);
				OnFinishCommand(success);
				
				if(_started)
				{
					OnReleaseResources();
					_resourcesReleased = true;					
				}
				
				if(this.Scope == CommandScope.Application)
					UnityEngine.Object.Destroy(this.gameObject);
				else
				{
					MonoLog.Log(MonoLogChannel.Core, "Destroing component " + this.GetType().Name);
					
					UnityEngine.Object.Destroy(this);
				}
			}
		}

		private void OnApplicationQuit()
		{
			_resourcesReleased = true;
		}
		
		protected override sealed void OnDestroy()
		{				
			MonoLog.Log(MonoLogChannel.Core, "Command " + this.GetType().Name + " has beed destroyed");
			
			if(!_resourcesReleased && _started)
			{
				try
				{
					OnReleaseResources();
				}
				catch(Exception e)
				{
					MonoLog.Log(MonoLogChannel.Core,"Error during release command resources", e);
				}
				
				_resourcesReleased = true;
			}
		}
		
		public void Terminate()
		{
			if(_running)
			{
				FinishCommand(_started);
			}
		}
	}
	
	public abstract class MonoCommand<T>:MonoCommand
		where T:MonoCommand<T>,new()
	{
		public MonoCommand()
		{
			AsyncToken = new AsyncToken<T>((T)this);
		}
		
		public AsyncToken<T> AsyncToken
		{
			get;
			private set;
		}
		
		protected sealed override void OnFinishCommand (bool success = true)
		{
			if(success)
				AsyncToken.FireResponse();
			else
				AsyncToken.FireFault();
		}
	}
}

