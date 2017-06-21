using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
	public class StateCommand:MonoCommand<StateCommand>
	{
	    protected override void OnStart(object[] args)
	    {
        }
	}
	
	public interface IStateMachineContainer
	{
		void Next(StateCommand previousState);
		
		GameObject GameObject
		{
			get;
		}
	}
	
	public interface IStateMachine
	{
		StateCommand State
		{
			get;
		}
		
		StateCommand ApplyState(Type type, params object[] args);
		
		T ApplyState<T>(params object[] args) where T:StateCommand,new();
		
		T Execute<T>(params object[] args) where T:StateCommand,new();
		
		StateCommand Execute(Type type, params object[] args);		
	}
	
	
	public sealed class StateFlow:IStateMachineContainer
	{
		private readonly List<NextStatePair> _stateFlow;
		private readonly IStateMachineContainer _container;
		private readonly StateMachine _stateMachine;
		
		public StateFlow(IStateMachineContainer container, StateMachine stateMachine)
		{
			_stateFlow = new List<NextStatePair>();
			_container = container;
			_stateMachine = stateMachine;
		}
		
		public sealed class NextStatePair
		{
			public readonly Type Success;
			public readonly Type Fail;
			public readonly Type Target;
			
			public NextStatePair(Type target, Type success, Type fail)
			{
				this.Target = target;
				this.Success = success;
				this.Fail = fail;
			}
			
			public NextStatePair(Type target, Type success)
			{
				this.Target = target;
				this.Success = success;
				this.Fail = success;
			}
		}
		
		public void Add(NextStatePair pair)
		{
			_stateFlow.Add(pair);
		}
		
		#region IStateMachineContainer implementation
		public void Next (StateCommand prevState)
		{
			NextStatePair pair = null;
			
			if(prevState != null)
			{
				pair = _stateFlow.Find(delegate(NextStatePair nextStatePair)
				{
					return nextStatePair.Target == prevState.GetType();	
				});
			}
				
			if(pair != null)			
			{
				if(prevState.FinishedWithSuccess)			
					_stateMachine.ApplyState(pair.Success, _container);
				else 
					_stateMachine.ApplyState(pair.Fail, _container);				
			}
		}

		public GameObject GameObject 
		{
			get 
			{
				return _container.GameObject;
			}
		}
		#endregion
	}
	
	public class StateMachine : IStateMachine
	{
		public event System.Action<StateMachine> Changed;
		
		private IStateMachineContainer _container;		
#if UNITY_EDITOR					
		public string[] StateHistory;
		
		private List<string> _stateHistory = new List<string>();
#endif		
		private readonly List<MonoCommand> _commands;

		private StateCommand _state;
		
		public StateCommand State
		{
			get
			{
				return _state;
			}
		}
			
		public StateMachine(IStateMachineContainer container)
		{
			_container = container;	
			_commands = new List<MonoCommand>();
		}
		
		private void OnStateFinished(StateCommand state)
		{						
			if(state == _state)
			{	
				_state = null;
				
				StopAllCommands();

				_container.Next( state );
			}
		}
		
		private StateCommand ChangeState(Type type, object[] args)
		{
			StateCommand prevState = _state;
			
			_state = null;
			
			if(prevState != null)		
			{
				prevState.Terminate();
						
				StopAllCommands();
			}

			_state = CreateState(type, args);

#if UNITY_EDITOR				
			_stateHistory.Add(type.Name);
			
			StateHistory = _stateHistory.ToArray();
#endif				
			_state.AsyncToken.AddResponder(new Responder<StateCommand>(OnStateFinished, OnStateFinished));			
			
			if(Changed != null)
				Changed(this);	
			
			return _state;
		}
		
		public StateCommand ApplyState(Type type, params object[] args)
		{			
			return ChangeState(type,args);						
		}
		
		public T ApplyState<T>(params object[] args) where T:StateCommand,new()
		{
			return ChangeState(typeof(T),args) as T;									
		}
		
		private StateCommand CreateState(Type type, object[] args)
		{
			return (StateCommand)MonoCommand.ExecuteOn(type, _container.GameObject, args);
		}
		
		private void StopAllCommands()
		{
			int maxCalls = _commands.Count;

			while(_commands.Count > 0)
			{
				maxCalls--;

				_commands[0].Terminate();

				if(maxCalls == 0)
					break;
			}

			if(_commands.Count != 0)
			{
				MonoLog.LogWarning(MonoLogChannel.Exceptions,"State machine does not finished all commands");
			}

			/*for(int i = _commands.Count-1; i >= 0; i--)
			{
				_commands[i].Terminate();
			}*/
		}
		
		public T Execute<T>(params object[] args) where T:StateCommand,new()
		{
			T command = MonoCommand.ExecuteOn<T>(_container.GameObject, args);
			
			_commands.Add(command);
			
			command.AsyncToken.AddResponder(new Responder<StateCommand>(delegate(StateCommand result)
			{
				_commands.Remove( result );
			},
			delegate(StateCommand result)
			{
				_commands.Remove( result );
			}
			));
			
			return command;
		}
		
		public StateCommand Execute(Type type, params object[] args)
		{
			StateCommand command = (StateCommand)MonoCommand.ExecuteOn(type, _container.GameObject,args);
			
			_commands.Add(command);

			command.AsyncToken.AddResponder(new Responder<StateCommand>(delegate(StateCommand result)
			{
				_commands.Remove( result );
			}));			
			
			return command;
		}
				
	}
	
}

