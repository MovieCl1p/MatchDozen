using System;
using UI;
using UnityEngine;

namespace Game.UI.PopUp
{
    public class FinishPopupController : UIPopUpController
    {
        [SerializeField]
        private tk2dUIItem _restartButton;
        
        protected override void OnStart()
        {
            base.OnStart();
            _restartButton.OnClick += RestartButtonClick;
        }

        private void RestartButtonClick()
        {
            Close();
        }
    }
}
