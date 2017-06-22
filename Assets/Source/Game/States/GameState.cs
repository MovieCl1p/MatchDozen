using Core;

namespace Game.States
{
    public class GameState : StateCommand
    {
        private GameSceneController _controller;

        public GameSceneController Controller
        {
            get
            {
                return _controller;
            }
        }
        
        protected override void OnStart(object[] args)
        {
            _controller = (GameSceneController)args[0];
        }
    }
}
