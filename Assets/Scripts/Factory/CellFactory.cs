
using System;
using Assets.Scripts.Game;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Factory
{
    public class CellFactory
    {
        private static Dictionary<int, string> _matchColors = new Dictionary<int, string>();
        private static Dictionary<int, string> _normalColor = new Dictionary<int, string>();

        static CellFactory()
        {
            _matchColors.Add(1, "fea9a6");
            _matchColors.Add(2, "dcddf3");
            _matchColors.Add(3, "e7f0bc");
            _matchColors.Add(4, "ffd6ba");
            _matchColors.Add(5, "83e5fa");
            _matchColors.Add(6, "fae1fd");
            _matchColors.Add(7, "5af1e5");
            _matchColors.Add(8, "ffe397");
            _matchColors.Add(9, "e7f6ff");
            _matchColors.Add(10, "cffdcb");
            _matchColors.Add(11, "fef4a9");
            _matchColors.Add(12, "ffabd3");

            _normalColor.Add(1, "f16b68");
            _normalColor.Add(2, "a0a2c8");
            _normalColor.Add(3, "b1c27c");
            _normalColor.Add(4, "f6997a");
            _normalColor.Add(5, "4daedb");
            _normalColor.Add(6, "dba8ea");
            _normalColor.Add(7, "32c4ad");
            _normalColor.Add(8, "f7aa5c");
            _normalColor.Add(9, "b0d0ff");
            _normalColor.Add(10, "90ea8c");
            _normalColor.Add(11, "edc96b");
            _normalColor.Add(12, "f46d95");
        }
        
        public static CellController GetCell(RectTransform parent)
        {
            var go = GameObject.Instantiate<CellController>(Resources.Load<CellController>("CellController"), parent);
            return go;
        }

        public static Color GetCellMatchColor(int count)
        {
            Color c;
            if (ColorUtility.TryParseHtmlString("#" + _matchColors[count], out c))
            {
                return c;
            }

            return Color.black;
        }

        public static Color GetCellColor(int count)
        {
            Color c;
            if(ColorUtility.TryParseHtmlString("#"+ _normalColor[count], out c))
            {
                return c;
            }

            return Color.black;
        }
    }
}