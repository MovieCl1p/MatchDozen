using System;
using System.Collections.Generic;
using Core;
using Game.Cells;
using Game.Cells.States;
using Game.Commands;
using Game.Commands.Arguments;
using Game.UI.PopUp;
using Parachute.Menu;
using UI;
using UnityEngine;

namespace Game.States
{
    public class GamePlayState : GameState
    {
        private bool[][] _field;
        private List<CellController> _cellControllers;
        
        private bool _canControl = true;

        protected override void OnStart(object[] args)
        {
            base.OnStart(args);
            
            Controller.StateMachine.Execute<GenerateLevelCommand>(Controller).AsyncToken.AddResponder(new Responder<StateCommand>(OnLevelGenerated));
        }

        private void OnLevelGenerated(StateCommand obj)
        {
            GenerateLevelCommand command = (GenerateLevelCommand)obj;
            _cellControllers = command.CellControllers;
            _field = command.Field;
            
            for (int i = 0; i < _cellControllers.Count; i++)
            {
                _cellControllers[i].Click += OnClickCell;
            }
        }

        private void OnClickCell(CellController cell)
        {
            if (!_canControl)
            {
                return;
            }

            CheckMatching(cell);
        }

        private void CheckMatching(CellController cell)
        {
            _canControl = false;
            CheckMatchingCommandArguments matchingArgs = new CheckMatchingCommandArguments(cell, _cellControllers);
            Controller.StateMachine.Execute<CheckMatchingCommand>(matchingArgs).AsyncToken.AddResponder(new Responder<StateCommand>(OnFinishCheckingMatchingSuccess, OnFinishCheckingMatchingFail));
        }

        private void OnFinishCheckingMatchingSuccess(StateCommand command)
        {
            CheckMatchingCommand com = (CheckMatchingCommand) command;

            CellController cell = com.MatchingCell;
            List<CellController> openSet = com.OpenSet;
            Controller.Model.Score += cell.Model.Count * openSet.Count;
            cell.Selected = true;
            
            for (int i = 0; i < openSet.Count; i++)
            {
                CellController currentCell = openSet[i];
                currentCell.Selected = true;
                _field[currentCell.Model.Position.x][currentCell.Model.Position.y] = true;
            }

            CellController last = openSet[openSet.Count - 1];
            StartDieQueue(last, openSet, () =>
            {
                cell.Model.Count++;
                cell.Selected = false;

                if (cell.Model.Count > Controller.Model.MaxScoreNumber)
                {
                    Controller.Model.MaxScoreNumber = cell.Model.Count;
                }

                CascadeCommandArguments args = new CascadeCommandArguments(_field, _cellControllers);
                Controller.StateMachine.Execute<CascadeCommand>(args)
                    .AsyncToken.AddResponder(new Responder<StateCommand>(OnCascadeFinish));
            });
        }

        private void OnFinishCheckingMatchingFail(StateCommand command)
        {
            _canControl = true;
        }
        
        private void StartDieQueue(CellController cell, List<CellController> openSet, Action OnComplete)
        {
            cell.StateMachine.ApplyState<CellDieState>(cell).AsyncToken.AddResponder(new Responder<StateCommand>(command =>
            {
                openSet.Remove(cell);
                cell.Click -= OnClickCell;

                _cellControllers.Remove(cell);

                if (openSet.Count > 0)
                {
                    CellController last = openSet[openSet.Count - 1];
                    StartDieQueue(last, openSet, OnComplete);
                }
                else
                {
                    if (OnComplete != null)
                    {
                        OnComplete();
                    }
                }
            }));
        }
        
        private void OnCascadeFinish(StateCommand command)
        {
            _canControl = true;
            PlaceElementsCommandArguments arg = new PlaceElementsCommandArguments(Controller.View.ElementsPlaceHolder, _field, _cellControllers, OnClickCell);
            Controller.StateMachine.Execute<PlaceElementsCommand>(arg).AsyncToken.AddResponder(new Responder<StateCommand>(Result));
        }

        private void Result(StateCommand stateCommand)
        {
            bool result = false;
            for (int i = 0; i < _cellControllers.Count; i++)
            {
                if (result)
                {
                    return;
                }

                int index = i;
                CheckMatchingCommandArguments matchingArgs = new CheckMatchingCommandArguments(_cellControllers[index], _cellControllers);
                Controller.StateMachine.Execute<CheckMatchingCommand>(matchingArgs).AsyncToken.AddResponder(new Responder<StateCommand>(
                    command =>
                    {
                        //success    
                        result = true;
                    }, 
                    command =>
                    {
                        if (index >= _cellControllers.Count - 1)
                        {
                            if (!result)
                            {
                                OnLose();
                            }
                        }
                    }));
            }
        }

        private void OnLose()
        {
            _canControl = false;
//            Controller.StateMachine.ApplyState<GameFinishState>(_cellControllers);
            UIManager.OpenPopUp<FinishPopupController>(Controller).Closed += OnClosed;
//            UIManager.Load<MenuSceneController>();
//            Controller.StateMachine.ApplyState<GameInitializeState>();
        }

        private void OnClosed(UIPopUpController uiPopUpController)
        {
            OnReleaseResources();
        }

        protected override void OnReleaseResources()
        {
            base.OnReleaseResources();

            if (_cellControllers != null)
            {
                for (int i = 0; i < _cellControllers.Count; i++)
                {
                    _cellControllers[i].Click -= OnClickCell;
                    GameObject.Destroy(_cellControllers[i].GameObject);
                }

                _cellControllers.Clear();
            }
            
            _field = null;
        }
    }
}
