
using Assets.Scripts.Game.Model;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class CellModel
    {
        private int _count;
        private GridPoint _gridPosition;
        private Vector3 _viewPosition;

        public event System.Action OnDataChanged;



        public CellModel(float speed)
        {
            MoveSpeed = speed;
            _count = 1;
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