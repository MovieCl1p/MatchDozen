using Assets.Scripts.Factory;
using System.Collections.Generic;
using UnityEngine;

namespace Gui.Screens.Game
{
    public class TopPanel : MonoBehaviour
    {
        public List<TopItem> List;
        private TopItem _current;

        protected void Start()
        {
            for (int i = 0; i < List.Count; i++)
            {
                List[i].NormalColor = CellFactory.GetCellColor(i + 1);
                List[i].SetCurrent(false);
            }
        }

        public void UpdatePanel(int maxScore)
        {
            return;
            if(_current != null)
            {
                _current.SetCurrent(false);
            }

            _current = List[maxScore - 1];
            _current.SetCurrent(true);
        }
    }
}