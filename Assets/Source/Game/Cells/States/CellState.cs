using Core;

namespace Game.Cells.States
{
    public class CellState : StateCommand
    {
        private CellController _cellController;

        protected override void OnStart(object[] args)
        {
            base.OnStart(args);
            _cellController = (CellController) args[0];
        }

        protected CellController Controller
        {
            get { return _cellController; }
        }
    }
}
