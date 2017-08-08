
using Assets.Scripts.Factory;
using Assets.Scripts.Game.Model;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class PlaceElementsCommand
    {
        private bool[][] Field;
        private List<CellController> CellControllers;
        private Action<CellController> OnClickAction;
        private RectTransform PlaceHolder;

        public PlaceElementsCommand(bool[][] field, List<CellController> cellControllers, Action<CellController> onClickAction, RectTransform holder)
        {
            Field = field;
            CellControllers = cellControllers;
            OnClickAction = onClickAction;
            PlaceHolder = holder;

            StartField();
        }

        private void StartField()
        {
            for (int i = 0; i < Field.Length; i++)
            {
                for (int j = 0; j < Field[i].Length; j++)
                {
                    if (Field[i][j])
                    {
                        CellController cellController = CellFactory.GetCell(PlaceHolder);
                        cellController.Model.GridPosition = new GridPoint(i, j);
                        cellController.Model.WorldsPosition = FieldUtils.GetWorldPosition(i, j);
                        cellController.Model.Count = UnityEngine.Random.Range(1, 5);

                        cellController.Init();
                        
                        cellController.MoveToPosition();

                        cellController.transform.localPosition = FieldUtils.GetWorldPosition(i, 5);

                        if (OnClickAction != null)
                        {
                            cellController.Click += OnClickAction;
                        }

                        CellControllers.Add(cellController);

                        Field[i][j] = false;
                    }
                }
            }
        }
    }
}