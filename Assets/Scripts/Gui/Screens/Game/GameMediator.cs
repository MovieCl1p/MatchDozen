
using System;
using strange.extensions.mediation.impl;
using Assets.Scripts.Game;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Factory;
using Assets.Scripts.Game.Model;

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
                _field[(int)currentCell.Model.GridPosition.X][(int)currentCell.Model.GridPosition.Y] = true;
            }

            CellController last = openSet[openSet.Count - 1];
            StartDieQueue(last, openSet, () =>
            {
                cell.Model.Count++;
                cell.Selected = false;
                cell.UpdateView();
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
            
            PlaceElementsCommand command = new PlaceElementsCommand(_field, _cellControllers, OnClickCell, View.CellPlaceHolder);
        }

        private void StartDieQueue(CellController cell, List<CellController> openSet, Action OnComplete)
        {
            cell.Die(command =>
            {
                openSet.Remove(cell);
                cell.Click -= OnClickCell;

                _cellControllers.Remove(cell);

                Destroy(cell.gameObject);
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
            Vector2 pivot = new Vector2(View.CellPlaceHolder.rect.x + 50, View.CellPlaceHolder.rect.y + 50);

            FieldUtils.SetPivot(pivot);

            for (int i = 0; i < _field.Length; i++)
            {
                for (int j = 0; j < _field[i].Length; j++)
                {
                    if (_field[i][j])
                    {
                        CellController cellController = CellFactory.GetCell(View.CellPlaceHolder);
                        
                        float dx = pivot.x + (100 * i);
                        float dy = pivot.y + (100 * j);

                        cellController.Model.GridPosition = new GridPoint(i, j);
                        cellController.Model.WorldsPosition = new Vector2(dx, dy);
                        cellController.Model.Count = UnityEngine.Random.Range(1, 5);

                        cellController.Init();

                        _cellControllers.Add(cellController);

                        _field[i][j] = false;
                    }
                }
            }
        }

        //public void CalcPhysicalPositions(int width, int height, Point offsetCoords)
        //{
        //    // setup constant if need
        //    FieldSize = new Point(width, height);
        //    if (_constantFieldSize != Point.Zero)
        //    {
        //        FieldSize = _constantFieldSize;
        //    }

        //    // calculate offsets for center
        //    var offsetForCenter = OffsetForCenter(width, height, offsetCoords);

        //    // reinitialize
        //    _physicalPositions = new Vector2[FieldSize.x, FieldSize.y];

        //    // get physical size
        //    var rectTransform = (RectTransform)transform;
        //    var rect = rectTransform.rect;
        //    var pivot = rectTransform.pivot;
        //    _layerTransform.localPosition = new Vector3(rect.min.x + rect.size.x * pivot.x, rect.min.y + rect.size.y * pivot.y);

        //    // calculate min scale factor, pivot offset and relative base size
        //    FieldRealSize = new Vector2(_baseCellSize.x * FieldSize.x + (_space * (FieldSize.x - 1)), _baseCellSize.y * FieldSize.y + (_space * (FieldSize.y - 1)));
        //    ScaleFactor = Mathf.Min(rect.width / FieldRealSize.x, rect.height / FieldRealSize.y);
        //    var pivotOffset = new Vector2((rect.width / ScaleFactor - FieldRealSize.x) * pivot.x, (rect.height / ScaleFactor - FieldRealSize.y) * pivot.y);
        //    var realSpace = _space * ScaleFactor;

        //    // initialize physical positions grid
        //    var minPosition = rect.min / ScaleFactor;
        //    var startPosition = new Vector2(minPosition.x + _baseCellSize.x / 2f + pivotOffset.x, minPosition.y + _baseCellSize.y / 2f + pivotOffset.y);
        //    for (var y = 0; y < FieldSize.y; y++)
        //    {
        //        for (var x = 0; x < FieldSize.x; x++)
        //        {
        //            _physicalPositions[x, y] = new Vector2(startPosition.x + (_baseCellSize.x + realSpace) * x, startPosition.y + (_baseCellSize.y + realSpace) * y) + offsetForCenter;
        //            if (x == 0 && y == 0)
        //            {
        //                _rootPosition = _physicalPositions[x, y];
        //            }
        //        }
        //    }
        //}
    }
}