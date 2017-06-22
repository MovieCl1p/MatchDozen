using Core;
using UnityEngine;

namespace Game.UI.TopPanel
{
    public class TopPanelElement : BaseMonoBehaviour
    {
        [SerializeField]
        private tk2dSprite _sprite;

        [SerializeField] private int _number;

        public int Number
        {
            get { return _number; }
        }

        public void SetActive(bool active)
        {
            string newSpriteName = "pl";
            int count = _number;
            if (count < 10)
            {
                newSpriteName += "0";
            }

            if (count > 12)
            {
                count = 12;
            }

            newSpriteName += count;
            _sprite.SetSprite(newSpriteName);
            
            if (active)
            {
                _sprite.color = Color.white;
            }
            else
            {
                _sprite.color = new Color32(138, 160, 189, 255);
            }
        }
    }
}
