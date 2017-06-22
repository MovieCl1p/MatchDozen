using TMPro;
using UI;
using UnityEngine;

namespace Game.Cells
{
    public class CellView : MGComponent
    {
        [SerializeField]
        private TextMeshPro _text;

        [SerializeField]
        private tk2dSprite _sprite;

        private bool _selected = false;
        private int _count = 1;

        public bool Selected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                SetSprite(_count, _selected);
            }
        }

        public void SetCount(int count)
        {
            _text.text = count.ToString();
        }

        public void SetSprite(int count, bool selected = false)
        {
            string newSpriteName = "pl";
            if (count < 10)
            {
                newSpriteName += "0";
            }

            if (count > 12)
            {
                count = 12;
            }

            newSpriteName += count;
            if (selected)
            {
                newSpriteName += "_on";
            }

            _sprite.SetSprite(newSpriteName);
            _count = count;
        }
        
        public void Die()
        {
            Destroy(gameObject);
        }

        protected override void OnStart()
        {
            base.OnStart();
            UIManager.CurrentSceneController.SceneInput.Add(this);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UIManager.CurrentSceneController.SceneInput.Remove(this);
        }
    }
}
