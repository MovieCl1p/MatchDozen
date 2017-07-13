
using Assets.Scripts.Game;
using UnityEngine;

namespace Assets.Scripts.Factory
{
    public class CellFactory
    {
        public static CellController GetCell(RectTransform parent)
        {
            var go = GameObject.Instantiate<CellController>(Resources.Load<CellController>("CellController"), parent);
            return go;
        }
    }
}