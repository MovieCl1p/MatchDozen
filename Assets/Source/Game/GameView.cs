using System;
using Core;
using Game.UI.TopPanel;
using TMPro;
using UnityEngine;

namespace Game
{
    public class GameView : BaseMonoBehaviour
    {
        public event Action OnMenuButtonClick;

        [SerializeField]
        private tk2dUICamera _camera;

        [SerializeField]
        private TextMeshPro _scores;

        [SerializeField]
        private Transform _elementPlaceholder;

        [SerializeField]
        private TopPanelController _topPanelController;

        [SerializeField]
        private tk2dUIItem _menuButton;

        private GameModel _model;
        
        public Transform ElementsPlaceHolder
        {
            get { return _elementPlaceholder; }
        }

        protected override void Start()
        {
            base.Start();
            tk2dUIManager.RegisterCamera(_camera);

            _menuButton.OnClick += MenuButtonClick;
        }

        private void MenuButtonClick()
        {
            if (OnMenuButtonClick != null)
            {
                OnMenuButtonClick();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            tk2dUIManager.UnregisterCamera(_camera);

            _model.OnScoreChanged -= OnScoreChanged;
            _model.OnMaxScoreNumberChanged -= OnMaxScoreNumberChanged;

            _menuButton.OnClick -= OnMenuButtonClick;
        }
        
        public void ApplyModel(GameModel model)
        {
            _model = model;
            if (_model != null)
            {
                _model.OnScoreChanged += OnScoreChanged;
                _model.OnMaxScoreNumberChanged += OnMaxScoreNumberChanged;
            }
        }

        private void OnMaxScoreNumberChanged(int maxScore)
        {
            _topPanelController.SetMaxScore(maxScore);
        }

        private void OnScoreChanged(int score)
        {
            _scores.text = score.ToString();
        }
    }
}
