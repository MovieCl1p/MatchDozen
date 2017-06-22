using System;
using System.Collections.Generic;
using Core;

namespace UI
{
	public abstract class UIControllerStrategy
	{
		internal UIController _controller;
		
		internal void Initialize()
		{
			OnInitialize();
		}
		
		protected virtual void OnInitialize(){}
		
		public virtual bool TriggerSuppressed
		{
			get
			{
				return false;
			}
		}
		
	}
	
	public abstract class UIControllerStrategyTyped<T>:UIControllerStrategy
		where T:UIController
	{
		public T Controller
		{
			get
			{
				return (T)_controller;
			}
			set
			{
				_controller = value;
			}
		}
	}
	
	public interface UIControllerStrategyFactory
	{
		UIControllerStrategy CreateStrategy(UIController controller);
	}
	
	public abstract class UIControllerTriggerAttribute:Attribute
	{
		public event System.Action<UIControllerTriggerAttribute> Finish;
		
		public abstract void OnControllerStart(UIController controller);
		
		protected void FinishTrigger()
		{
			if(Finish != null)
				Finish(this);
		}
	}
	
	public abstract class UIController : MonoScheduledBehaviour, UIControllerStrategyFactory
	{		
		
		public UIController() 
		{
		}
		
		private object[] _args;
		
		public object[] Args
		{
			get
			{
				return _args;
			}
		}
		
		protected UIControllerStrategy _strategy;
		
		protected virtual UIControllerStrategy CreateStrategy()
		{
			return null;
		}
		
		public UIControllerStrategy CreateStrategy(UIController controller)
		{
			return CreateStrategy();
		}
		
		
		
		protected override void Update()
		{
			base.Update();
			
			OnUpdate();
		}
		
		protected virtual void OnUpdate() {}
	
		protected virtual void OnInitialize()
		{
		}

		void Awake()
		{
			_args = UIManager.PollArgs(this);
			
			OnInitialize();							
		}
		
		protected override sealed void Start()
		{													
			Queue<UIControllerTriggerAttribute> triggers = new Queue<UIControllerTriggerAttribute>();
						
			foreach(Attribute attribute in this.GetType().GetCustomAttributes(true))
			{
				if(attribute is UIControllerTriggerAttribute)
					triggers.Enqueue( (UIControllerTriggerAttribute) attribute );
			}

			
			UIManager.RegisterController(this);			
			
			ProcessTriggers( triggers );
		}

        protected virtual void OnTriggersFinished()
		{
		    if (_args != null && _args.Length > 0)
		    {
		        OnStart(_args);
		    }
		    else
		    {
		        OnStart();
		    }	
			
			OnAfterStart();				
		}
		
		private void ProcessTriggers(Queue<UIControllerTriggerAttribute> triggers)
		{
		    if (triggers.Count > 0)
		    {
		        UIControllerTriggerAttribute trigger = triggers.Dequeue();

		        trigger.Finish += delegate(UIControllerTriggerAttribute obj)
		        {
		            ProcessTriggers(triggers);
		        };

		        trigger.OnControllerStart(this);
		    }
		    else
		    {
		        OnTriggersFinished();
		    }
		}
		
		protected sealed override  void OnDestroy()
		{			
			base.OnDestroy();
			
			UIManager.UnregisterController();
		}
				
		internal virtual void OnAfterStart()
		{
			
		}
		
		protected virtual void OnStart() { }

		protected virtual void OnStart(object[] args) { }
		
		public T OpenPopUp<T>()
			where T:UIPopUpController
		{
			return OpenPopUp<T>(new object[]{});
		}
				
		public T OpenPopUp<T>(params object[] args)
			where T:UIPopUpController
		{
			return UIManager.OpenPopUp<T>(this,args);
		}
		
		public void LoadScene<T>()
		{
			LoadScene<T>(new object[]{});
		}
		
		public void LoadScene<T>(params object[] args)
		{
			MonoLog.Log(MonoLogChannel.UI,string.Format( "Loading scene {0}", typeof(T)));
			
			UIManager.Load(typeof(T),args);
		}

	}
	
	/*public abstract class UIController<TModel,TView>:UIController
		where TModel:new()
		where TView:UIView<UIController<TModel,TView>,TModel>
	{
		[SerializedField]
		public TModel Model;
		
		public TView View;
	}*/
}

