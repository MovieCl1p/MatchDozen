using System;
using Core;
using Game.States;
using Parachute.Menu;
using UI;
using UnityEngine;

namespace Game
{
    [UISceneName("Game")]
    public class GameSceneController : UISceneController, IStateMachineContainer, IUIPopUpControllerHandler
    {
        [SerializeField]
        private GameView _view;

        private GameModel _model;
        
        private StateMachine _stateMachine;
        private StateFlow _stateFlow;

        public GameSceneController()
        {
            _model = new GameModel();
            _stateMachine = new StateMachine(this);
            _stateFlow = new StateFlow(this, _stateMachine);
            _stateFlow.Add(new StateFlow.NextStatePair(typeof(GameInitializeState), typeof(GameReadyState)));
            _stateFlow.Add(new StateFlow.NextStatePair(typeof(GameReadyState), typeof(GamePlayState)));
        }

        public void Next(StateCommand previousState)
        {
            _stateFlow.Next(previousState);
        }

        public GameObject GameObject
        {
            get { return gameObject; }
        }

        public GameView View
        {
            get
            {
                return this._view;
            }
        }

        public StateMachine StateMachine
        {
            get
            {
                return this._stateMachine;
            }
        }

        public GameModel Model
        {
            get { return _model; }
        }

        public void PopUpDidFinish(UIPopUpController controller)
        {
            _stateMachine.ApplyState<GameInitializeState>(this);
            _stateMachine.State.AsyncToken.AddResponder(new Responder<StateCommand>(OnGameInitialized));
        }

        protected override void OnStart()
        {
            base.OnStart();
            _stateMachine.ApplyState<GameInitializeState>(this);
            _stateMachine.State.AsyncToken.AddResponder(new Responder<StateCommand>(OnGameInitialized));
        }

        private void OnGameInitialized(StateCommand command)
        {
            View.OnMenuButtonClick += OnMenuButtonClick;

            View.ApplyModel(_model);
            _model.Score = 0;
            _model.MaxScoreNumber = 4;
        }

        private void OnMenuButtonClick()
        {
//            UIManager.Load<MenuSceneController>();
        }
    }
}
