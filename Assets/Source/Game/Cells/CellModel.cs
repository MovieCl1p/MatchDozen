using System;
using Mechanics.Defense;
using UnityEngine;

namespace Game.Cells
{
    public class CellModel
    {
        private int _count;
        private MapPoint _position;
        private Vector3 _viewPosition;

        public event Action OnDataChanged;

        public CellModel(float speed)
        {
            MoveSpeed = speed;
            _count = 1;
        }

        public Vector3 ViewPosition
        {
            get { return _viewPosition; }
            set
            {
                _viewPosition = value;
                CallDataChanged();
            }
        }

        public MapPoint Position
        {
            get { return _position; }
            set
            {
                _position = value;
                CallDataChanged();
            }
        }

        public int Count
        {
            get { return _count; }
            set
            {
                _count = value;
                CallDataChanged();
            }
        }
        
        public float MoveSpeed { get; set; }

        private void CallDataChanged()
        {
            if (OnDataChanged != null)
            {
                OnDataChanged();
            }
        }
    }
}
