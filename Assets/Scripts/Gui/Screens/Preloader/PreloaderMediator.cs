using AssetControl;
using strange.extensions.mediation.impl;

namespace Gui.Screens
{
    public class PreloaderMediator : Mediator
    {
        [Inject]
        public PreloaderView View { get; set; }

        public override void OnRegister()
        {
            View.ChangeView(ViewNames.MainMenu);
        }
    }
}