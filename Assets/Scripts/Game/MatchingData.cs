
using System.Collections.Generic;

namespace Assets.Scripts.Game
{
    public class MatchingData
    {
        public CellController MatchingCell { get; set; }
        public List<CellController> OpenSet { get; set; }
    }
}