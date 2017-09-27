
using System;
using Assets.Scripts.Game;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Game.Model;

namespace Assets.Scripts.Factory
{
    public class CellFactory
    {
        private static Dictionary<int, string> _matchColors = new Dictionary<int, string>();
        private static Dictionary<int, string> _normalColor = new Dictionary<int, string>();

        private static Dictionary<int, CellColor> _color = new Dictionary<int, CellColor>();

        static CellFactory()
        {
            _color.Add(1, new CellColor("00fbd9", "03deff", "00fdd7", "00daff"));
            _color.Add(2, new CellColor("5464f8", "7655fb", "2b3aff", "512eff"));
            _color.Add(3, new CellColor("1ddf6d", "1ee4a4", "00fd43", "00ff9a"));
            _color.Add(4, new CellColor("f7556d", "f96954", "ff2b41", "ff452d"));
            _color.Add(5, new CellColor("cd5ba0", "d15378", "ff109b", "ff0e41"));
            _color.Add(6, new CellColor("70c344", "51c645", "37ff01", "0cff03"));
            _color.Add(7, new CellColor("118fff", "1753ff", "0572ff", "0730ff"));
            _color.Add(8, new CellColor("861ede", "be1fe3", "6800fd", "ca00ff"));
            _color.Add(9, new CellColor("ff8a28", "ffc22c", "ff6910", "ffbc13"));
            _color.Add(10, new CellColor("f33524", "f77322", "ff1208", "ff4408"));
            _color.Add(11, new CellColor("f9bd37", "f6f836", "ffb416", "feff14"));
            _color.Add(12, new CellColor("e847e5", "f836a7", "ff16fd", "ff1494"));


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
            RectTransform rect = go.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(FieldUtils.Size.x, FieldUtils.Size.y);
            return go;
        }

        public static Color GetCellBotMatchColor(int count)
        {
            Color c;
            if (ColorUtility.TryParseHtmlString("#" + _color[count].BotMatchColor, out c))
            {
                return c;
            }

            return Color.black;
        }

        public static Color GetCellTopMatchColor(int count)
        {
            Color c;
            if (ColorUtility.TryParseHtmlString("#" + _color[count].TopMatchColor, out c))
            {
                return c;
            }

            return Color.black;
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

        public static Color GetCellBotColor(int count)
        {
            Color c;
            if (ColorUtility.TryParseHtmlString("#" + _color[count].BotColor, out c))
            {
                return c;
            }

            return Color.black;
        }

        public static Color GetCellTopColor(int count)
        {
            Color c;
            if (ColorUtility.TryParseHtmlString("#" + _color[count].TopColor, out c))
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