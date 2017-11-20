using Core.Gui.ViewManager;
using UnityEngine;

namespace Core
{
    public class AppManager
    {
        private static AppManager _instance;

        public AppManager()
        {
            //Localization = new LocalizationManager();
        }

        public IViewManager ViewManager { get; private set; }
        //public LocalizationManager Localization { get; set; }

        public void Init(GameObject root)
        {
            ViewManager = root.GetComponent<ViewManager>();
            ViewManager.Init();
        }

        public static AppManager GetInstance()
        {
            return _instance ?? (_instance = new AppManager());
        }
    }
}
