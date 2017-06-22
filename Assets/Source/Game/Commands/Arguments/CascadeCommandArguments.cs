using System.Collections.Generic;
using Game.Cells;

namespace Game.Commands.Arguments
{
    public class CascadeCommandArguments
    {
        private bool[][] _field;
        private List<CellController> _cellControllers;

        public CascadeCommandArguments(bool[][] field, List<CellController> cellControllers)
        {
            _field = field;
            _cellControllers = cellControllers;
        }

        public bool[][] Field
        {
            get { return _field; }
        }

        public List<CellController> CellControllers
        {
            get
            {
                return _cellControllers;
            }
        }
    }
}
