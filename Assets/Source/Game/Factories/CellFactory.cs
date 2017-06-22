
using Game.Cells;
using UnityEngine;

namespace Game.Factories
{
    public class CellFactory
    {

        public static CellController GetCell(Transform placeHolder)
        {
            CellController result = GameObject.Instantiate(Resources.Load<CellController>("CellController"));
            result.CachedTransform.parent = placeHolder;
            return result;
        }
    }
}
