using AssetControl;
using Core;
using Core.Audio;
using Core.Gui.ViewManager;
using Core.ResourceManager;
using Data;
using Model;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using Services.Interfaces;
using Signals;
using UnityEngine;

namespace Commands
{
    public class StartCommand : Command
    {
        [Inject(ContextKeys.CONTEXT_VIEW)]
        public GameObject ContextView { get; set; }

        [Inject]
        public GameSettingsModel GameSettingsModel { get; set; }

        [Inject]
        public SetQualitySignal SetQualitySignal { get; set; }
        
        [Inject]
        public IPlayerService PlayerService { get; set; }

        private IViewManager _viewManager;

        public override void Execute()
        {
            ResourcesCache.SetupResourcesCache(AssetsNames.PreloaderLinks, true);
            ResourcesCache.SetupResourcesCache(AssetsNames.UiAssetsLinks, true);
            ResourcesCache.SetupResourcesCache(AssetsNames.GameElementAssetsLinks, true);

            AppManager.GetInstance().Init(ContextView);
            _viewManager = AppManager.GetInstance().ViewManager;

            RegisterViews();

            _viewManager.SetView(ViewNames.SplashView);
            _viewManager.AddView(ViewNames.PreloaderView);

            injectionBinder.Bind<IViewManager>().To(_viewManager);

            DeviceCapabilities.InitAdvertisingId();
            PlayerService.Initialize();

            AudioManager.DefaultSection = AssetsNames.AudioSection;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            
            SetQualitySignal.Dispatch(GameSettingsModel.QualityPreset);
        }

        private void RegisterViews()
        {
            _viewManager.RegisterView(Layers.ScreenLayer, ViewNames.SplashView);
            _viewManager.RegisterView(Layers.ScreenLayer, ViewNames.PreloaderView);
            _viewManager.RegisterView(Layers.ScreenLayer, ViewNames.MainMenu);
            _viewManager.RegisterView(Layers.ScreenLayer, ViewNames.GameView);
        }
    }
}