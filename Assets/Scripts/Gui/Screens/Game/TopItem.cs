using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gui.Screens.Game
{
    public class TopItem : MonoBehaviour
    {
        public Image Image;
        public Color NormalColor;

        public void SetCurrent(bool isCurrent)
        {
            Image.color = isCurrent ? Color.black : NormalColor;
        }
    }
}