using Commands;
using Commands.Startup;
using Core.Plugins.Strange;
using Gui.Screens;
using Model;
using strange.extensions.command.api;
using strange.extensions.injector.api;
using strange.extensions.mediation.api;
using Services;
using Services.Interfaces;
using Signals;

namespace Configs
{
    public class GameConfig : IConfig
    {
        [Inject]
        public IMediationBinder MediationBinder { get; set; }

        [Inject]
        public ICommandBinder CommandBinder { get; set; }

        [Inject]
        public IInjectionBinder InjectionBinder { get; set; }

        public void Configure()
        {
            InjectionBinds();
            ModelBind();
            ViewBind();
            CommandsBind();
            ServicesBind();
        }

        private void ModelBind()
        {
            InjectionBinder.Bind<GameSettingsModel>().ToSingleton();
            InjectionBinder.Bind<PlayerModel>().ToSingleton();
        }

        private void ViewBind()
        {
            MediationBinder.BindView<PreloaderView>().ToMediator<PreloaderMediator>();
        }

        private void CommandsBind()
        {
            CommandBinder.Bind<StartSignal>().InSequence()
                .To<StartCommand>()
                .To<LoadResourcesCommand>()
                .To<InitializeSettingsCommand>()
                .To<CreateDataBaseCommand>()
                .To<GetPlayerInfoCommand>()
                .To<RecalculateBoostersAvailablilityCommand>()
                .To<ShowFirstViewCommand>().Once();

            CommandBinder.Bind<SetQualitySignal>().To<SetQualityCommand>();
        }

        private void ServicesBind()
        {
            InjectionBinder.Bind<IPlayerService>().To<PlayerService>().ToSingleton();
        }

        private void InjectionBinds()
        {
        }
    }
}