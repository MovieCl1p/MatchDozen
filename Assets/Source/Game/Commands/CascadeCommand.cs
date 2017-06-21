using System.Collections.Generic;
using Core;
using Game.Cells;
using Game.Commands.Arguments;
using Mechanics.Defense;
using UnityEngine;

namespace Game.Commands
{
    public class CascadeCommand : StateCommand
    {
        private List<CellController> _cellControllers;
        private bool[][] _field;

        protected override void OnStart(object[] args)
        {
            base.OnStart(args);

            CascadeCommandArguments arg = (CascadeCommandArguments) args[0];
            _cellControllers = arg.CellControllers;
            _field = arg.Field;
            
            for (int i = 0; i < _cellControllers.Count; i++)
            {
                CellController cell = _cellControllers[i];
                
                _field[cell.Model.Position.x][cell.Model.Position.y] = true;

                MapPoint newPosition = GetLowestPosition(cell.Model.Position);
                _field[newPosition.x][newPosition.y] = false;

                if (!newPosition.Equals(cell.Model.Position))
                {
                    cell.SetPosition(newPosition);
                    cell.SetViewPosition(new Vector3(newPosition.x, newPosition.y, 0));
                    cell.MoveToPosition();
                }
            }

            FinishCommand();
        }

        private MapPoint GetLowestPosition(MapPoint position)
        {
            if (position.y == 0)
            {
                return position;
            }

            int safeCount = 10;
            MapPoint lowestPosition = position;
            while (true)
            {
                if (lowestPosition.y <= 0)
                {
                    break;
                }
                
                lowestPosition = lowestPosition + MapPoint.down;
                if (lowestPosition.y >= 0)
                {
                    if (!_field[lowestPosition.x][lowestPosition.y])
                    {
                        lowestPosition = lowestPosition + MapPoint.up;
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

        private CellController GetCellByPosition(MapPoint position)
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
