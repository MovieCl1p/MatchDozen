
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
            _model = new CellModel(40);
            _model.OnDataChanged += OnModelDataChanged;

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

        public Vector3 DieDirection { get; set; }

        public bool Selected
        {
            get { return _view.Selected; }
            set { _view.Selected = value; }
        }
        
        protected void OnDestroy()
        {
            _model.OnDataChanged -= OnModelDataChanged;
            _view.Tap -= OnTap;
            _view.Die();
        }

        public void MoveToPosition()
        {
            gameObject.AddComponent<MoveToCommand>().Init(_model.ViewPosition, _model.MoveSpeed);
        }

        private void OnTap()
        {
            Debug.Log("Tap, controller");
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
                transform.localPosition = FieldUtils.GetPosition(_model.Position);
            }
        }

        public void SetPosition(Vector2 vector2)
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

        public void Die(Action<object> p)
        {
        }
    }
}