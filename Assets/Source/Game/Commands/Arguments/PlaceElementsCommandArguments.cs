using System;
using System.Collections.Generic;
using Game.Cells;
using UnityEngine;

namespace Game.Commands.Arguments
{
    public class PlaceElementsCommandArguments
    {
        private bool[][] _field;
        private List<CellController> _cellControllers;
        private Action<CellController> _onClickAction;
        private Transform _placeHolder;

        public PlaceElementsCommandArguments(Transform elementsPlaceHolder, bool[][] field, List<CellController> cellControllers, Action<CellController> onClickAction)
        {
            _placeHolder = elementsPlaceHolder;
            _field = field;
            _cellControllers = cellControllers;
            _onClickAction = onClickAction;
        }

        public bool[][] Field
        {
            get { return _field; }
        }

        public List<CellController> CellControllers
        {
            get
            {
                return _cellControllers;
            }
        }

        public Action<CellController> OnClickAction
        {
            get { return _onClickAction; }
        }

        public Transform PlaceHolder
        {
            get { return _placeHolder; }
        }
    }
}
