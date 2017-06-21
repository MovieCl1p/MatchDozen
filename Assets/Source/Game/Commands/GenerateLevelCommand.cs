using System;
using System.Collections.Generic;
using Core;
using Game.Cells;
using Game.Commands.Arguments;
using Game.Factories;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Commands
{
    public class GenerateLevelCommand : StateCommand
    {
        private bool[][] _field;
        private List<CellController> _cellControllers;

        public List<CellController> CellControllers
        {
            get { return _cellControllers; }
        }

        public bool[][] Field
        {
            get { return _field; }
        }

        protected override void OnStart(object[] args)
        {
            base.OnStart(args);

            _cellControllers = new List<CellController>();
                        _field = new bool[5][];
            for (int i = 0; i < _field.Length; i++)
            {
                _field[i] = new bool[5];
                for (int j = 0; j < _field[i].Length; j++)
                {
                    _field[i][j] = true;
                }
            }

            GameSceneController sceneController = (GameSceneController) args[0];

            PlaceElementsCommandArguments arg = new PlaceElementsCommandArguments(sceneController.View.ElementsPlaceHolder, _field, _cellControllers, null);
            sceneController.StateMachine.Execute<PlaceElementsCommand>(arg).AsyncToken.AddResponder(new Responder<StateCommand>(OnPlaceElementFinished));
        }

        private void OnPlaceElementFinished(StateCommand stateCommand)
        {
            FinishCommand();
        }
    }
}
