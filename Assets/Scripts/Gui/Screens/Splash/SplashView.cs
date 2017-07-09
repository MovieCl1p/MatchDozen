using Core.Gui.ViewManager;

namespace Gui.Screens
{
    public class SplashView : ViewBase
    {
        protected override void Start()
        {
            base.Start();
            CloseView();
        }
    }
}