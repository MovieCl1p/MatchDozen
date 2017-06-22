using System;
using UnityEngine;

namespace Parachute.Menu
{
    public class MenuSceneView : MonoBehaviour
    {
        [SerializeField]
        private tk2dUIItem _playButton;

        [SerializeField]
        private tk2dUICamera _camera;

        public event Action OnPlayClick;

        protected void Start()
        {
            tk2dUIManager.RegisterCamera(_camera);
            _playButton.OnClick += OnPlayButtonClick;
        }

        protected void OnDestroy()
        {
            tk2dUIManager.UnregisterCamera(_camera);
        }

        private void OnPlayButtonClick()
        {
            if (OnPlayClick != null)
            {
                OnPlayClick();
            }
        }
    }
}
