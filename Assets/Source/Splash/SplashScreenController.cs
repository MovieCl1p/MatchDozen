using App;
using App.States;
using Core;
using Parachute.Menu;
using UI;

namespace Parachute.Splash
{
    [UISceneName("Splash")]
    public sealed class SplashScreenController : UISceneController
    {
        protected override void OnStart()
        {
            base.OnStart();
            AppController.Instance.StateMachine.Changed += OnAppControllerStateChanged;
            AppController.Instance.StateMachine.ApplyState<AppInitializeState>(AppController.Instance);
        }

        private void OnAppControllerStateChanged(StateMachine stateMachine)
        {
            if (stateMachine.State is App.States.AppReadyState)
            {
                LoadScene<MenuSceneController>();
            }
        }
    }
}
