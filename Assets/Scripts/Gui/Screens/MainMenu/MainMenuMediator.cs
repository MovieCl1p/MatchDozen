using System;
using strange.extensions.mediation.impl;
using AssetControl;

namespace Assets.Scripts.Gui.Screens.MainMenu
{
    public class MainMenuMediator : Mediator
    {
        [Inject]
        public MainMenuView View { get; set; }

        public override void OnRegister()
        {
            View.PlayBtn.onClick.AddListener(OnPlayClick);
        }

        private void OnPlayClick()
        {
            View.ChangeView(ViewNames.GameView);
        }
    }
}