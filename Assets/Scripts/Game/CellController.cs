
using Assets.Scripts.Game.Model;
using System;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class CellController : MonoBehaviour
    {
        [SerializeField]
        private CellView _view;
        
        private CellModel _model;

        public event System.Action<CellController> Click;
        
        private void Awake()
        {
            _model = new CellModel(300);
            //_model.OnDataChanged += OnModelDataChanged;

            _view.Tap += OnTap;
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

        public GridPoint DieDirection { get; set; }

        public bool Selected
        {
            get { return _view.Selected; }
            set
            {
                _view.Selected = value;
                
            }
        }
        
        protected void OnDestroy()
        {
            //_model.OnDataChanged -= OnModelDataChanged;
            _view.Tap -= OnTap;
            _view.Die();
        }

        public void MoveToPosition()
        {
            gameObject.AddComponent<MoveToCommand>().Init(_model.WorldsPosition, _model.MoveSpeed);
        }

        private void OnTap()
        {
            if (Click != null)
            {
                Click(this);
            }
        }

        //private void OnModelDataChanged()
        //{
        //    if (_view != null)
        //    {
        //        _view.SetCount(_model.Count);
        //        transform.localPosition = _model.Position;
        //    }
        //}

        public void SetWorldPosition(Vector2 vector2)
        {
            _model.WorldsPosition = vector2;
        }

        public void SetGridPosition(GridPoint position)
        {
            _model.GridPosition = position;
        }

        public void SetCount(int count)
        {
            _model.Count = count;
        }

        public void Die(Action<object> p)
        {
            Vector3 diePosition = FieldUtils.GetWorldPosition(DieDirection.X + Model.GridPosition.X, DieDirection.Y + Model.GridPosition.Y);

            gameObject.AddComponent<MoveToCommand>().Init(diePosition, _model.MoveSpeed, p);
        }
        
        public void Init()
        {
            UpdateView();
        }

        public void UpdateView()
        {
            transform.localPosition = new Vector3(Model.WorldsPosition.x, Model.WorldsPosition.y, 0);
            _view.SetCount(_model.Count);
        }
    }
}