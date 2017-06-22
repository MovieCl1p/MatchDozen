using System.Collections.Generic;
using System.Linq;
using Core;
using UnityEngine;

namespace Game.UI.TopPanel
{
    public class TopPanelController : BaseMonoBehaviour
    {
        [SerializeField]
        private List<TopPanelElement> _elements;

        private TopPanelElement _currentElement;

        protected override void Start()
        {
            for (int i = 0; i < _elements.Count; i++)
            {
                _elements[i].SetActive(false);
            }
        }

        public void SetMaxScore(int maxScore)
        {
            TopPanelElement nextElement = _elements.FirstOrDefault(element => element.Number == maxScore);
            if (nextElement != null)
            {
                nextElement.SetActive(true);
                if (_currentElement != null)
                {
                    _currentElement.SetActive(false);
                }
                
                _currentElement = nextElement;
            }
        }
    }
}
