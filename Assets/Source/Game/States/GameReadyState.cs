

namespace Game.States
{
    public class GameReadyState : GameState
    {
        protected override void OnStart(object[] args)
        {
            base.OnStart(args);
            FinishCommand();
        }
    }
}
