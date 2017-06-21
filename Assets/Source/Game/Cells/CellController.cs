using System;
using Core;
using Cowabunga.UI.Commands;
using Game.Cells.Commands.Arguments;
using Game.Cells.States;
using Mechanics.Defense;
using UI;
using UnityEngine;

namespace Game.Cells
{
    public class CellController : BaseMonoBehaviour, IStateMachineContainer
    {
        [SerializeField]
        private CellView _view;

        private StateMachine _stateMachine;

        private CellModel _model;
        
        public event Action<CellController> Click;
        
        public StateMachine StateMachine
        {
            get { return _stateMachine; }
        }

        private void Awake()
        {
            _model = new CellModel(40);
            _model.OnDataChanged += OnModelDataChanged;

            _view.Tap += OnTap;

            _stateMachine = new StateMachine(this);
            _stateMachine.ApplyState<CellIdleState>(this);
        }
        
        public CellModel Model
        {
            get { return _model; }
        }

        public bool Checked
        {
            get; set;
        }
        
        public CellView View
        {
            get { return _view; }
        }

        public GameObject GameObject
        {
            get { return gameObject; }
        }

        public Vector3 DieDirection { get; set; }

        public bool Selected
        {
            get { return _view.Selected; }
            set { _view.Selected = value; }
        }

        public void Next(StateCommand previousState)
        {

        }

        protected override void OnReleaseResources()
        {
            base.OnReleaseResources();
            _model.OnDataChanged -= OnModelDataChanged;
            _view.Tap -= OnTap;
            _view.Die();
        }

        public void MoveToPosition()
        {
            MoveToCommandArgument argument = new MoveToCommandArgument(CachedTransform, _model.ViewPosition, _model.MoveSpeed);
            _stateMachine.Execute<MoveToCommand>(argument);
        }

        private void OnTap(MGComponent mgComponent, Vector3 vector3)
        {
            if (Click != null)
            {
                Click(this);
            }
        }

        private void OnModelDataChanged()
        {
            if (_view != null)
            {
                _view.SetCount(_model.Count);
                _view.SetSprite(_model.Count);
            }
        }

        public void SetPosition(MapPoint vector2)
        {
            _model.Position = vector2;
        }

        public void SetViewPosition(Vector3 position)
        {
            _model.ViewPosition = FieldUtils.GetPosition(position);
        }

        public void SetCount(int count)
        {
            _model.Count = count;
        }
    }
}
