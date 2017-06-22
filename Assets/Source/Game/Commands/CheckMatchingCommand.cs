using System.Collections.Generic;
using Core;
using Game.Cells;
using Game.Commands.Arguments;
using Mechanics.Defense;
using UnityEngine;

namespace Game.Commands
{
    public class CheckMatchingCommand : StateCommand
    {
        private List<CellController> _cellControllers;

        public CellController MatchingCell { get; private set; }
        public List<CellController> OpenSet { get; private set; }

        protected override void OnStart(object[] args)
        {
            base.OnStart(args);
            CheckMatchingCommandArguments arg = (CheckMatchingCommandArguments) args[0];

            MatchingCell = arg.Cell;
            _cellControllers = arg.CellControllers;

            OpenSet = new List<CellController>();
            int matchCount = MatchingCell.Model.Count;

            OpenSet.Add(MatchingCell);
            GetNeighbor(MatchingCell, matchCount, OpenSet);
            OpenSet.Remove(MatchingCell);

            if (OpenSet.Count > 0)
            {
                FinishCommand(true);
            }
            else
            {
                FinishCommand(false);
            }
        }

        private void GetNeighbor(CellController cell, int matchCount, List<CellController> openSet)
        {
            CellController uperElement = GetCellByPosition(cell.Model.Position + MapPoint.up);
            if (uperElement != null && uperElement.Model.Count == matchCount && !openSet.Contains(uperElement))
            {
                uperElement.DieDirection = Vector3.down;
                openSet.Add(uperElement);
                GetNeighbor(uperElement, matchCount, openSet);
            }

            CellController lowerElement = GetCellByPosition(cell.Model.Position + MapPoint.down);
            if (lowerElement != null && lowerElement.Model.Count == matchCount && !openSet.Contains(lowerElement))
            {
                lowerElement.DieDirection = Vector3.up;
                openSet.Add(lowerElement);
                GetNeighbor(lowerElement, matchCount, openSet);
            }

            CellController leftElement = GetCellByPosition(cell.Model.Position + MapPoint.left);
            if (leftElement != null && leftElement.Model.Count == matchCount && !openSet.Contains(leftElement))
            {
                leftElement.DieDirection = Vector3.right;
                openSet.Add(leftElement);
                GetNeighbor(leftElement, matchCount, openSet);
            }

            CellController rightElement = GetCellByPosition(cell.Model.Position + MapPoint.right);
            if (rightElement != null && rightElement.Model.Count == matchCount && !openSet.Contains(rightElement))
            {
                rightElement.DieDirection = Vector3.left;
                openSet.Add(rightElement);
                GetNeighbor(rightElement, matchCount, openSet);
            }
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
