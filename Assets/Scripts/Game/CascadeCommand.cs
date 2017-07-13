
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

                _field[(int)cell.Model.Position.x][(int)cell.Model.Position.y] = true;

                Vector2 newPosition = GetLowestPosition(cell.Model.Position);
                _field[(int)newPosition.x][(int)newPosition.y] = false;

                if (!newPosition.Equals(cell.Model.Position))
                {
                    cell.SetPosition(newPosition);
                    cell.SetViewPosition(new Vector3(newPosition.x, newPosition.y, 0));
                    cell.MoveToPosition();
                }
            }

            onCascadeFinish();
        }

        private Vector2 GetLowestPosition(Vector2 position)
        {
            if (position.y == 0)
            {
                return position;
            }

            int safeCount = 10;
            Vector2 lowestPosition = position;
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

            return lowestPosition;
        }

        private CellController GetCellByPosition(Vector2 position)
        {
            CellController result = null;

            for (int i = 0; i < _cellControllers.Count; i++)
            {
                if (_cellControllers[i].Model.Position.Equals(position))
                {
                    result = _cellControllers[i];
                    break;
                }
            }

            return result;
        }
    }
}