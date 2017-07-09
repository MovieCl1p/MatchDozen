using strange.extensions.dispatcher.eventdispatcher.api;

namespace Services.Interfaces
{
    public interface IPlayerService
    {
        IEventDispatcher Dispatcher { get; set; }

        void Initialize();

        void GetPlayer(string socialId = null, string accountId = null, int age = 0);
    }
}