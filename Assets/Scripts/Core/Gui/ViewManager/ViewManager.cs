using System;
using System.Collections.Generic;
using Core.ResourceManager;
using UnityEngine;

namespace Core.Gui.ViewManager
{
    public struct ViewStruct
    {
        public string LayerId;
        public string SectionId;
        public string Linker;

        public ViewStruct(string layerId, string sectionId, string linker)
        {
            LayerId = layerId;
            SectionId = sectionId;
            Linker = linker;
        }
    }

    public class ViewManager : MonoBehaviour, IViewManager
    {
        public List<ViewLayer> Layers;

        private Dictionary<string, ViewLayer> _uiLayersDict;
        private Dictionary<string, ViewStruct> _viewDictionary;

        public void Init()
        {
            _viewDictionary = new Dictionary<string, ViewStruct>();
            _uiLayersDict = new Dictionary<string, ViewLayer>();

            foreach (var layer in Layers)
            {
                _uiLayersDict[layer.name] = layer;
            }
        }

        public void RegisterView(string layerId, string viewId, string section = null, string linker = null)
        {
            //TODO: temporary, need remove it
            if (section == null)
                section = "GuiWindows";

            _viewDictionary[viewId] = new ViewStruct(layerId, section, linker);

        }

        public ViewBase SetView(string viewId, object options = null, bool isDebug = false)
        {
            return SetView(viewId, GetLayerByViewId(viewId), options, isDebug);
        }

        public ViewBase SetViewToLayer(string viewId, string layerId, object options = null, bool isDebug = false)
        {
            return SetView(viewId, GetLayerById(layerId), options, isDebug);
        }

        private ViewBase SetView(string viewId, ViewLayer layer, object options = null, bool isDebug = false)
        {
            if (layer.Current == null)
            {
                CreateView(viewId, options, isDebug, layer);
            }
            else
            {
                layer.AddViewToLine(viewId, options, isDebug, 0);
                layer.Current.CloseView();
            }

            return layer.Current;
        }

        public void AddView(string viewId, object options = null, bool isDebug = false)
        {
            AddView(viewId, GetLayerByViewId(viewId), options, isDebug);
        }

        public void AddViewToLayer(string viewId, string layerId, object options = null, bool isDebug = false)
        {
            AddView(viewId, GetLayerById(layerId), options, isDebug);
        }

        private void AddView(string viewId, ViewLayer layer, object options = null, bool isDebug = false)
        {
            if (layer.Current == null)
            {
                CreateView(viewId, options, isDebug, layer);
            }
            else
            {
                layer.AddViewToLine(viewId, options);
            }
        }

        public void RemoveView(ViewBase view)
        {
            var layer = view.Layer;
            layer.RemoveCurrentView();

            string linker = _viewDictionary[view.name].Linker;
            if (linker != null)
                ResourcesCache.Clean(linker, true);

            var viewVo = layer.GetNextView();
            if (viewVo != null)
            {
                SetView(viewVo.ViewId, layer, viewVo.Options);
            }
            else
            {
                layer.ClearHisory();
                layer.OnAllViewRemovedSignal.Dispatch();

                var lowerLayer = GetLowerLayer(layer);
                if (lowerLayer && !lowerLayer.Current.IsManualActivation)
                {
                    //TODO: timeout
                    //TimeoutUtils.AddTimeout(lowerLayer.Current.Activate, 0.05f);
                }
            }
        }

        private ViewLayer GetLowerLayer(ViewLayer layer)
        {
            var index = Layers.IndexOf(layer);

            if (index > 0)
            {
                for (int i = index - 1; i >= 0; i--)
                {
                    if (Layers[i].Current != null && Layers[i].IsGui)
                        return Layers[i];
                }
            }
            return null;
        }

        private void CreateView(string viewId, object options, bool isDebug, ViewLayer layer)
        {
            if (!_viewDictionary.ContainsKey(viewId))
            {
                throw new Exception("Can't find view with such id " + viewId);
            }

            string section = _viewDictionary[viewId].SectionId;
            string linker = _viewDictionary[viewId].Linker;
            if (linker != null)
            {
                if (!ResourcesCache.IsResourceLinkLoaded(linker))
                {
                    ResourcesCache.SetupResourcesCache(linker, true);
                }

            }

            var view = Instantiate(ResourcesCache.GetObject<ViewBase>(section, viewId));
            view.Options = options;
            view.IsDebug = isDebug;
            view.Layer = layer;
            view.gameObject.SetActive(true);
            view.gameObject.name = viewId;
            view.transform.SetParent(layer.transform, false);
            layer.AddViewToHistory(viewId, options);
            layer.AddView(view);

            var lowerLayer = GetLowerLayer(layer);
            if (lowerLayer && layer.IsGui)
                lowerLayer.Current.DeActivate();
        }

        public ViewLayer GetLayerById(string layerId)
        {
            if (!_uiLayersDict.ContainsKey(layerId))
            {
                throw new Exception("Can't find layer with such id " + layerId);
            }

            return _uiLayersDict[layerId];
        }

        public ViewLayer GetLayerByViewId(string viewId)
        {
            if (!_viewDictionary.ContainsKey(viewId))
            {
                throw new Exception("Can't find view with such id " + viewId);
            }
            string layerKey = _viewDictionary[viewId].LayerId;
            return GetLayerById(layerKey);
        }

        public bool IsViewOpened(string viewId)
        {
            for (int i = 0; i < Layers.Count; i++)
            {
                if (Layers[i].Current != null && Layers[i].Current.name.Equals(viewId))
                    return true;
            }


            return false;
        }

        public string GetCurrentViewId(string layerId)
        {
            var layer = GetLayerById(layerId);
            if (layer.Current != null)
            {
                return layer.Current.name;
            }

            return null;
        }

        public ViewBase GetTopView()
        {
            for (int i = Layers.Count - 1; i >= 0; i--)
            {
                if (Layers[i].Current != null && Layers[i].IsGui)
                    return Layers[i].Current;
            }

            return null;
        }

        public void OpenPreviouseView(ViewBase view)
        {
            var layer = view.Layer;
            var viewVo = layer.GetPreviouseView();
            if (viewVo == null)
                layer.Current.CloseView();
            else
            {

                //remove current view from history
                layer.RemoveLastViewFromHistory();

                //remove previouse view from history which we are going to add again
                layer.RemoveLastViewFromHistory();

                SetView(viewVo.ViewId, viewVo.Options);
            }

        }

        public MonoBehaviour Current
        {
            get { return this; }
        }
    }
}
