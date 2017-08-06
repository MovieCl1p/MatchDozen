
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Game
{
    public class CellView : MonoBehaviour
    {
        public event Action Tap;

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
            }
        }

        protected void Awake()
        {
            _brn.onClick.AddListener(Click);
        }

        public void SetCount(int count)
        {
            _text.text = count.ToString();
        }
        
        public void Die()
        {
            Destroy(gameObject);
        }
        
        public void Click()
        {
            Debug.Log("Click view");
            if(Tap != null)
            {
                Tap();
            }
        }
    }
}