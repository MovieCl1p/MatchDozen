
using Assets.Scripts.Game.Model;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class CascadeCommand
    {
        private bool[][] _field;
        private List<CellController> _cellControllers;
        private Action onCascadeFinish;

        public CascadeCommand(bool[][] field, List<CellController> cellControllers, Action onCascadeFinish)
        {
            _field = field;
            _cellControllers = cellControllers;
            this.onCascadeFinish = onCascadeFinish;
            
            for (int i = 0; i < _cellControllers.Count; i++)
            {
                CellController cell = _cellControllers[i];

                _field[cell.Model.GridPosition.X][cell.Model.GridPosition.Y] = true;

                GridPoint newPosition = GetLowestPosition(cell.Model.GridPosition);
                _field[newPosition.X][newPosition.Y] = false;

                if (!newPosition.Equals(cell.Model.GridPosition))
                {
                    cell.Model.GridPosition = newPosition;
                    cell.Model.WorldsPosition = FieldUtils.GetWorldPosition(newPosition.X, newPosition.Y);
                    cell.MoveToPosition();
                }
            }

            onCascadeFinish();
        }

        private GridPoint GetLowestPosition(GridPoint position)
        {
            if (position.Y == 0)
            {
                return position;
            }

            int safeCount = 10;
            Vector2 lowestPosition = new Vector2(position.X, position.Y);
            while (true)
            {
                if (lowestPosition.y <= 0)
                {
                    break;
                }

                lowestPosition = lowestPosition + Vector2.down;
                if (lowestPosition.y >= 0)
                {
                    if (!_field[(int)lowestPosition.x][(int)lowestPosition.y])
                    {
                        lowestPosition = lowestPosition + Vector2.up;
                        break;
                    }
                }

                if (--safeCount <= 0)
                {
                    break;
                }
            }

            return new GridPoint((int)lowestPosition.x, (int)lowestPosition.y);
        }

        private CellController GetCellByPosition(GridPoint position)
        {
            CellController result = null;

            for (int i = 0; i < _cellControllers.Count; i++)
            {
                if (_cellControllers[i].Model.GridPosition.Equals(position))
                {
                    result = _cellControllers[i];
                    break;
                }
            }

            return result;
        }
    }
}