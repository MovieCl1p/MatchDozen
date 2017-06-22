using Core;
using Game;
using UI;
using UnityEngine;

namespace Parachute.Menu
{
    [UISceneName("Menu")]
    public class MenuSceneController : UISceneController
    {
        [SerializeField]
        private MenuSceneView _view;


        protected override void OnStart()
        {
            base.OnStart();
            _view.OnPlayClick += GoToGame;
        }
        
        public void GoToGame()
        {
            LoadScene<GameSceneController>();
        }
    }
}
