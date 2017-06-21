using Core;
using Game.Commands;

namespace Game.States
{
    public class GameInitializeState : GameState
    {
        protected override void OnStart(object[] args)
        {
            base.OnStart(args);
            FinishCommand();
        }
    }
}
