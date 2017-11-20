using strange.extensions.signal.impl;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Gui.ViewManager
{
    public class ViewLayer : MonoBehaviour
    {
        public ViewBase Current { get; set; }
        private readonly List<View> _inLineViews;

        public Signal OnAddViewSignal { get; private set; }
        public Signal OnStartRemoveViewSignal { get; private set; }
        public Signal OnAllViewRemovedSignal { get; private set; }

        public bool IsGui;

        private List<View> _history;
        public ViewLayer()
        {
            _history = new List<View>();
            _inLineViews = new List<View>();
            OnAddViewSignal = new Signal();
            OnStartRemoveViewSignal = new Signal();
            OnAllViewRemovedSignal = new Signal();
        }

        public void AddView(ViewBase view)
        {
            Current = view;
            Current.StartTransitionOutSignal.AddOnce(OnStartHideView);
            OnAddViewSignal.Dispatch();
        }

        public void AddChild(GameObject child)
        {
            child.transform.SetParent(transform, false);
        }

        private void OnStartHideView()
        {
            OnStartRemoveViewSignal.Dispatch();
        }

        public void AddViewToLine(string viewId, object options = null, bool isDebug = false, int index = -1)
        {
            if (index == -1)
                _inLineViews.Add(new View(viewId, options));
            else
                _inLineViews.Insert(index, new View(viewId, options));
        }

        public View GetNextView()
        {
            if (_inLineViews.Count > 0)
            {
                View lastView = _inLineViews[0];
                _inLineViews.Remove(lastView);
                return lastView;
            }
            else
            {
                return null;
            }
        }

        public void RemoveCurrentView()
        {
            if (Current != null)
            {
                Current.StartTransitionOutSignal.RemoveListener(OnStartHideView);
                Destroy(Current.gameObject);
                Current = null;
            }
        }

        public int CountViewsInLine
        {
            get { return _inLineViews.Count; }
        }

        public void ClearLayer()
        {
            RemoveCurrentView();

            int childs = transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                Destroy(transform.GetChild(i).gameObject);
            }

            _inLineViews.Clear();
            ClearHisory();
            OnAllViewRemovedSignal.Dispatch();
        }

        public void AddViewToHistory(string viewId, object options = null)
        {
            _history.Add(new View(viewId, options));
        }

        public void RemoveLastViewFromHistory()
        {
            if (_history.Count > 0)
                _history.RemoveAt(_history.Count - 1);
        }

        public void ClearHisory()
        {
            _history = new List<View>();
        }

        public View GetPreviouseView()
        {
            if (_history.Count < 2)
                return null;

            return _history[_history.Count - 2];
        }
    }
}
