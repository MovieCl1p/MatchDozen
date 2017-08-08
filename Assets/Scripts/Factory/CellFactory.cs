
using System;
using Assets.Scripts.Game;
using UnityEngine;
using System.Collections.Generic;

namespace Assets.Scripts.Factory
{
    public class CellFactory
    {
        static CellFactory()
        {
            _colors.Add(1, new Color(254 / 255f, 169 / 255f,  166 / 255f));
            _colors.Add(2, new Color(220 / 255f, 221 / 255f,  243 / 255f));
            _colors.Add(3, new Color(231 / 255f,  240 / 255f,  188 / 255f));
            _colors.Add(4, new Color(255 / 255f,  214 / 255f,  186 / 255f));
            _colors.Add(5, new Color(131 / 255f,  229 / 255f,  250 / 255f));
            _colors.Add(6, new Color(250 / 255f,  225 / 255f,  253 / 255f));
            _colors.Add(7, new Color(90 / 255f,  241 / 255f,  229 / 255f));
            _colors.Add(8, new Color(255 / 255f,  227 / 255f,  151 / 255f));
            _colors.Add(9, new Color(231 / 255f,  246 / 255f,  255 / 255f));
            _colors.Add(10, new Color(207 / 255f,  253 / 255f,  203 / 255f));
            _colors.Add(11, new Color(254 / 255f,  244 / 255f,  169 / 255f));
            _colors.Add(12, new Color(255f / 255,  171 / 255f,  211 / 255f));

        }
        
        public static CellController GetCell(RectTransform parent)
        {
            var go = GameObject.Instantiate<CellController>(Resources.Load<CellController>("CellController"), parent);
            return go;
        }

        private static Dictionary<int, Color> _colors = new Dictionary<int, Color>();

        public static Color GetCellColor(int count)
        {
            return _colors[count];
        }
    }
}