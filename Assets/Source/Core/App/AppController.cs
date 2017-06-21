using System;
using App.States;
using Core;
using Parachute.Loader;
using UnityEngine;
using RoomRumble.UI;
using UI;

namespace App
{
	public sealed class AppController : MonoSingleton<AppController>, IStateMachineContainer, Observer
	{
		private readonly StateFlow _stateFlow;
		private readonly StateMachine _stateMachine;
		
		public readonly AppModel Model;
		
		public StateMachine StateMachine
		{
			get
			{
				return _stateMachine;
			}
		}

	    public AppController()
	    {
	        try
	        {
	            Model = AppModel.Load(AppModel.CONFIG_NAME);
	        }
	        catch (Exception e)
	        {
	            MonoLog.Log(MonoLogChannel.AppController, e);
	            Model = new AppModel();
	        }

	        Model.AddObserver(this);

	        _stateMachine = new StateMachine(this);
	        _stateFlow = new StateFlow(this, _stateMachine);
	        _stateFlow.Add(new StateFlow.NextStatePair(typeof (AppInitializeState), typeof (AppReadyState)));

	        LoadConfigs();
	    }


	    public void LoadConfigs()
		{
			if(!AppConfig.Load(AppModel.Instance))
				AppConfig.LoadDefault(AppModel.Instance);

			#if UNITY_EDITOR
			if (AppConfig.RuntimeSaveConfig)
			{
				try
				{
					AppConfig.Game.Save();
					AppConfig.Levels.Save();
                }
				catch(Exception e)
				{
					MonoLog.Log(MonoLogChannel.AppController,"Unable to save configs",e);
				}
			}
			#endif
		}

		protected override void Init ()
		{
			UIManager.LoadingSceneType = typeof(LoaderSceneController);		
			UIManager.PopUpFactory = new PopUpFactory();
			Application.targetFrameRate = 60;						
		}
		
		#region IStateMachineContainer implementation
		public void Next(StateCommand previousState)
		{
			_stateFlow.Next(previousState);
		}
		public GameObject GameObject 
		{
			get
			{
				return this.gameObject;
			}
		}
		#endregion
	
		#region Observer implementation
		public void OnObjectChanged (Observable observable)
		{
			try
			{
				Model.Save();
				Model.Commit();
			}
			catch(Exception e)
			{
				MonoLog.Log(MonoLogChannel.AppController,e);
			}
		}
		#endregion
	}
}