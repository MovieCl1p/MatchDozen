
using System.Collections.Generic;
using Game.Cells;

namespace Game.Commands.Arguments
{
    public class CheckMatchingCommandArguments
    {
        public CellController Cell { get; set; }
        public List<CellController> CellControllers { get; set; }

        public CheckMatchingCommandArguments(CellController cell, List<CellController> cellControllers)
        {
            Cell = cell;
            CellControllers = cellControllers;
        }
    }
}
