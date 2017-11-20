using UnityEngine;

namespace Core.Gui.ViewManager
{
    public interface IViewManager
    {
        void Init();
        void RegisterView(string layerId, string viewId, string section = null, string linker = null);
        ViewBase SetView(string viewId, object options = null, bool isDebug = false);
        ViewBase SetViewToLayer(string viewId, string layerId, object options = null, bool isDebug = false);

        void AddView(string viewId, object options = null, bool isDebug = false);
        void AddViewToLayer(string viewId, string layerId, object options = null, bool isDebug = false);
        void RemoveView(ViewBase view);
        void OpenPreviouseView(ViewBase view);
        ViewLayer GetLayerByViewId(string viewId);
        ViewLayer GetLayerById(string layerId);
        string GetCurrentViewId(string layerId);
        ViewBase GetTopView();
        bool IsViewOpened(string viewId);
        MonoBehaviour Current { get; }
    }
}
