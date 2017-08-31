
using Assets.Scripts.Factory;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game
{
    public class CellView : MonoBehaviour
    {
        public event Action Tap;

        [SerializeField]
        private Image _image;

        [SerializeField]
        private Text _text;

        [SerializeField]
        private Button _brn;

        private bool _selected = false;
        private int _count = 1;

        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                _brn.enabled = !value;
                if(value)
                {
                    _image.color = CellFactory.GetCellMatchColor(_count);
                }
                
            }
        }

        protected void Awake()
        {
            _brn.onClick.AddListener(Click);
        }

        public void SetCount(int count)
        {
            _count = count;
            _text.text = count.ToString();
            _image.color = CellFactory.GetCellColor(count);
        }
        
        public void Die()
        {
            Destroy(gameObject);
        }
        
        public void Click()
        {
            if(Tap != null)
            {
                Tap();
            }
        }
    }
}