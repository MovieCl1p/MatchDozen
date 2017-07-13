
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class CheckMatchingCommand
    {
        private CellController cell;
        private List<CellController> _cellControllers;
        private System.Action<MatchingData> success;
        private System.Action<MatchingData> fail;

        public CellController MatchingCell { get; private set; }
        public List<CellController> OpenSet { get; private set; }

        public CheckMatchingCommand(CellController cell, List<CellController> cellControllers, System.Action<MatchingData> onSuccess, System.Action<MatchingData> onFail)
        {
            this.cell = cell;
            _cellControllers = cellControllers;
            this.success = onSuccess;
            this.fail = onFail;

            MatchingCell = cell;
            _cellControllers = cellControllers;

            OpenSet = new List<CellController>();
            int matchCount = MatchingCell.Model.Count;

            OpenSet.Add(MatchingCell);
            GetNeighbor(MatchingCell, matchCount, OpenSet);
            OpenSet.Remove(MatchingCell);

            MatchingData data = new MatchingData();
            data.OpenSet = OpenSet;
            data.MatchingCell = MatchingCell;

            if (OpenSet.Count > 0)
            {
                success(data);
            }
            else
            {
                fail(data);
            }
        }

        private void GetNeighbor(CellController cell, int matchCount, List<CellController> openSet)
        {
            CellController uperElement = GetCellByPosition(cell.Model.Position + Vector2.up);
            if (uperElement != null && uperElement.Model.Count == matchCount && !openSet.Contains(uperElement))
            {
                uperElement.DieDirection = Vector3.down;
                openSet.Add(uperElement);
                GetNeighbor(uperElement, matchCount, openSet);
            }

            CellController lowerElement = GetCellByPosition(cell.Model.Position + Vector2.down);
            if (lowerElement != null && lowerElement.Model.Count == matchCount && !openSet.Contains(lowerElement))
            {
                lowerElement.DieDirection = Vector3.up;
                openSet.Add(lowerElement);
                GetNeighbor(lowerElement, matchCount, openSet);
            }

            CellController leftElement = GetCellByPosition(cell.Model.Position + Vector2.left);
            if (leftElement != null && leftElement.Model.Count == matchCount && !openSet.Contains(leftElement))
            {
                leftElement.DieDirection = Vector3.right;
                openSet.Add(leftElement);
                GetNeighbor(leftElement, matchCount, openSet);
            }

            CellController rightElement = GetCellByPosition(cell.Model.Position + Vector2.right);
            if (rightElement != null && rightElement.Model.Count == matchCount && !openSet.Contains(rightElement))
            {
                rightElement.DieDirection = Vector3.left;
                openSet.Add(rightElement);
                GetNeighbor(rightElement, matchCount, openSet);
            }
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