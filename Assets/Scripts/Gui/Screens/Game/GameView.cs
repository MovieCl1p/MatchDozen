using System;
using Core.Gui.ViewManager;
using UnityEngine;
using UnityEngine.UI;
using Gui.Screens.Game;

namespace Assets.Scripts.Gui.Screens.Game
{
    public class GameView : ViewBase
    {
        public RectTransform CellPlaceHolder;
        public Text ScoreLabel;
        public TopPanel Panel;

        private int _maxScore = 0;

        public void UpdateTopPanel(int elementScore, int score)
        {
            ScoreLabel.text = score.ToString();

            if(elementScore > _maxScore)
            {
                Panel.UpdatePanel(elementScore);
                _maxScore = elementScore;
            }
        }
    }
}