using Core;
using Cowabunga.UI.Commands;
using Game.Cells.Commands.Arguments;
using UI;
using UnityEngine;

namespace Game.Cells.States
{
    public class CellDieState : CellState
    {
        protected override void OnStart(object[] args)
        {
            base.OnStart(args);
            
            Vector3 diePosition = Controller.Model.ViewPosition + Controller.DieDirection * 4;
            MoveToCommandArgument argument = new MoveToCommandArgument(Controller.CachedTransform, diePosition, Controller.Model.MoveSpeed);
            Controller.StateMachine.Execute<MoveToCommand>(argument).AsyncToken.AddResponder(new Responder<StateCommand>(OnDieMovementFinish));
        }
        
        private void OnDieMovementFinish(StateCommand obj)
        {
            Destroy(Controller.GameObject);
            FinishCommand();
        }
    }
}
