
using System;
using strange.extensions.mediation.impl;
using Assets.Scripts.Game;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Factory;

namespace Assets.Scripts.Gui.Screens.Game
{
    public class GameMediator : Mediator
    {
        [Inject]
        public GameView View { get; set; }

        private bool[][] _field;
        private List<CellController> _cellControllers;

        private bool _canControl = true;

        public override void OnRegister()
        {
            GenerateField();

        }

        private void GenerateField()
        {
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

            StartField();

            for (int i = 0; i < _cellControllers.Count; i++)
            {
                _cellControllers[i].Click += OnClickCell;
            }
        }

        private void OnClickCell(CellController cell)
        {
            Debug.Log("OnClickCell");
            if (!_canControl)
            {
                return;
            }

            CheckMatching(cell);
        }

        private void CheckMatching(CellController cell)
        {
            _canControl = false;
            CheckMatchingCommand command = new CheckMatchingCommand(cell, _cellControllers, OnFinishCheckingMatchingSuccess, OnFinishCheckingMatchingFail);

        }

        private void OnFinishCheckingMatchingSuccess(MatchingData com)
        {
            CellController cell = com.MatchingCell;
            List<CellController> openSet = com.OpenSet;
            //Controller.Model.Score += cell.Model.Count * openSet.Count;
            cell.Selected = true;

            for (int i = 0; i < openSet.Count; i++)
            {
                CellController currentCell = openSet[i];
                currentCell.Selected = true;
                _field[(int)currentCell.Model.Position.x][(int)currentCell.Model.Position.y] = true;
            }

            CellController last = openSet[openSet.Count - 1];
            StartDieQueue(last, openSet, () =>
            {
                cell.Model.Count++;
                cell.Selected = false;

                //if (cell.Model.Count > Model.MaxScoreNumber)
                //{
                //    Model.MaxScoreNumber = cell.Model.Count;
                //}
                CascadeCommand cascade = new CascadeCommand(_field, _cellControllers, OnCascadeFinish);
            });
        }

        private void OnCascadeFinish()
        {
            _canControl = true;
            
            //Controller.StateMachine.Execute<PlaceElementsCommand>(arg).AsyncToken.AddResponder(new Responder<StateCommand>(Result));
        }

        private void StartDieQueue(CellController cell, List<CellController> openSet, Action OnComplete)
        {
            cell.Die(command =>
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
            });
        }

        private void OnFinishCheckingMatchingFail(MatchingData command)
        {
            _canControl = true;
        }

        private void StartField()
        {
            for (int i = 0; i < _field.Length; i++)
            {
                for (int j = 0; j < _field[i].Length; j++)
                {
                    if (_field[i][j])
                    {
                        CellController cellController = CellFactory.GetCell(View.CellPlaceHolder);
                        cellController.SetPosition(new Vector2(i, j));
                        cellController.SetCount(UnityEngine.Random.Range(1, 5));
                        //cellController.MoveToPosition();

                        
                        
                        _cellControllers.Add(cellController);

                        _field[i][j] = false;
                    }
                }
            }
        }
    }
}