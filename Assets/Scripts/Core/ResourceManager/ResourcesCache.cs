using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Linq;
using Core.ResourceManager.RALExtension;

namespace Core.ResourceManager
{
    public class ResourcesCache : MonoBehaviour
    {
        #region FIELDS

        private static readonly List<ResourceItem> _dynamicData = new List<ResourceItem>();
        private static readonly List<AssetBundle> _assetBundles = new List<AssetBundle>();
        private static MonoBehaviour _monoBehaviour;
        private static ResourceItem _staticData;

        #endregion FIELDS

        #region METHODS

        /// <summary>
        /// Is resource already loaded
        /// </summary>
        /// <param name="assetConfigName">Resource name</param>
        /// <returns></returns>
        public static bool IsResourceLinkLoaded(string assetConfigName)
        {
            if (_staticData != null && _staticData.Name == assetConfigName)
            {
                return true;
            }

            for (var i = 0; i < _dynamicData.Count; i++)
            {
                if (_dynamicData[i].Name == assetConfigName)
                {
                    return true;
                }
            }

            return false;
        }

        public static void Clean(string assetConfigName = null, bool cleanUnused = false)
        {
            if (assetConfigName == null)
            {
                _staticData = null;
            }
            else
            {
                for (var i = _dynamicData.Count - 1; i >= 0; i--)
                {
                    var data = _dynamicData[i];
                    if (data.Name == assetConfigName)
                    {
                        _dynamicData.RemoveAt(i);
                    }
                }
            }

            if (cleanUnused)
                Resources.UnloadUnusedAssets();
        }

        public static List<Object> GetAllObjects(string section)
        {
            return GetAllObjects<Object>(section);
        }

        public static List<T> GetAllObjects<T>(string section) where T : Object
        {
            var result = new List<T>();
            if (_staticData != null && _staticData.IsDone && _staticData.LinkerObject != null)
            {
                var subResult = _staticData.LinkerObject.GetAllObject<T>(section);
                if (subResult != null)
                {
                    result.AddRange(subResult);
                }
            }

            for (var i = 0; i < _dynamicData.Count; i++)
            {
                var data = _dynamicData[i];
                if (!data.IsDone)
                {
                    continue;
                }

                if (data.LinkerObject == null)
                {
                    Debug.LogWarning("LinkerObject is empty: " + data.Name + " type: " + typeof(T));
                    continue;
                }

                var subResult = data.LinkerObject.GetAllObject<T>(section);
                if (subResult != null)
                {
                    result.AddRange(subResult);
                }
            }

            if (result.Count == 0)
            {
                Debug.LogWarning("ResourcesCache: Not found objects by section: " + section + " type: " + typeof(T));
            }

            return result;
        }

        public static Object GetObject(string sectionName, string namePrefab)
        {
            return GetObject<Object>(sectionName, namePrefab);
        }

        public static T GetObject<T>(string sectionName, string namePrefab) where T : Object
        {
            // try get object from static
            if (_staticData != null && _staticData.IsDone)
            {
                var result = _staticData.LinkerObject.GetObject<T>(sectionName, namePrefab);
                if (result != null)
                {
                    return result;
                }
            }

            // try get object from dynamic
            if (_dynamicData != null && _dynamicData.Count > 0)
            {
                for (var i = 0; i < _dynamicData.Count; i++)
                {
                    var data = _dynamicData[i];
                    if (!data.IsDone)
                    {
                        continue;
                    }

                    var result = data.LinkerObject.GetObject<T>(sectionName, namePrefab);
                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            // not found
            if (Application.isEditor)
            {
                Debug.LogFormat("Can not find it: {0}, name: {1}", sectionName, namePrefab);
            }

            return default(T);
        }

        public static T GetObjectByComponent<T>(string sectionName, string namePrefab)
        {
            var gameObject = GetObject<GameObject>(sectionName, namePrefab);

            if (gameObject == null)
                throw new Exception("Can't find prefab: " + namePrefab + " in the section: " + sectionName);

            return gameObject.GetComponent<T>();
        }

        public static bool IsFindSection(string section)
        {
            if (_staticData != null && _staticData.IsDone && _staticData.LinkerObject.GetObject(section) != null)
            {
                return true;
            }

            if (_dynamicData != null && _dynamicData.Count > 0)
            {
                for (var i = 0; i < _dynamicData.Count; i++)
                {
                    var data = _dynamicData[i];
                    if (!data.IsDone)
                    {
                        continue;
                    }

                    if (data.LinkerObject.GetObject(section) != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool IsObjectLoaded<T>(string section, string namePrefab) where T : Object
        {
            if (_staticData != null && _staticData.IsDone)
            {
                var result = _staticData.LinkerObject.GetObject(section, namePrefab);
                if (result != null)
                {
                    return true;
                }
            }

            if (_dynamicData != null && _dynamicData.Count > 0)
            {
                for (var i = 0; i < _dynamicData.Count; i++)
                {
                    var data = _dynamicData[i];
                    if (!data.IsDone)
                    {
                        continue;
                    }

                    var result = data.LinkerObject.GetObject<T>(section, namePrefab);
                    if (result != null)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static void SetupResourcesCache(string assetConfigName, bool isAppendLoading, Action<bool, float> callback)
        {
            // initialize mono behavior
            if (_monoBehaviour == null)
            {
                _monoBehaviour = InitMonoBehaviour();
            }

            // work asynchronous
            if (!isAppendLoading)
            {
                // in static
                if (_staticData == null || _staticData.Name != assetConfigName)
                {
                    // other load
                    Clean();
                    _monoBehaviour.StartCoroutine(AsyncLoad(assetConfigName, false, callback));
                    return;
                }

                if (_staticData.IsDone)
                {
                    callback(true, 1f);
                }
                else
                {
                    _staticData.CallbackList.Add(callback);
                }
            }

            // in dynamic
            for (var i = 0; i < _dynamicData.Count; i++)
            {
                var data = _dynamicData[i];
                if (data.Name != assetConfigName)
                {
                    continue;
                }

                // data already load
                if (data.IsDone)
                {
                    callback(true, 1f);
                }

                // block thread for load
                data.CallbackList.Add(callback);
            }

            _monoBehaviour.StartCoroutine(AsyncLoad(assetConfigName, true, callback));
        }

        public static bool SetupResourcesCache(string assetConfigName = "RemoteAssetLinker", bool isAppendLoading = false)
        {
            // work sync
            if (!isAppendLoading)
            {
                // in static
                if (_staticData == null || _staticData.Name != assetConfigName)
                {
                    // check if new assets real
                    var dataObject = Load<RemoteAssetLinker>(assetConfigName);
                    if (dataObject == null)
                    {
                        // not found
                        return false;
                    }

                    // make new
                    Clean();
                    _staticData = new ResourceItem();
                    _staticData.Name = assetConfigName;
                    _staticData.LinkerObject = dataObject;
                    _staticData.Progress = 1f;
                    _staticData.IsDone = true;
                    return true;
                }

                // block thread for load
                _staticData.LinkerObject = Load<RemoteAssetLinker>(assetConfigName);
                _staticData.IsDone = true;
                return true;
            }

            // in dynamic
            for (var i = 0; i < _dynamicData.Count; i++)
            {
                var data = _dynamicData[i];
                if (data.Name != assetConfigName)
                {
                    continue;
                }

                // data already load
                if (data.IsDone)
                {
                    return true;
                }

                // block thread for load
                data.LinkerObject = Load<RemoteAssetLinker>(assetConfigName);
                data.IsDone = true;
                return true;
            }

            // load new dynamic
            var dynamicDataObject = Load<RemoteAssetLinker>(assetConfigName);
            if (dynamicDataObject == null)
            {
                // not found
                return false;
            }

            // make new
            var resourceItem = new ResourceItem();
            resourceItem.Name = assetConfigName;
            resourceItem.LinkerObject = dynamicDataObject;
            resourceItem.Progress = 1f;
            resourceItem.IsDone = true;
            _dynamicData.Add(resourceItem);

            return true;
        }

        private static IEnumerator AsyncLoad(string assetConfigName, bool isAppendLoading, Action<bool, float> process)
        {
            var asyncLoad = AsyncLoad<RemoteAssetLinker>(assetConfigName);
            if (asyncLoad == null)
            {
                process(false, -1f);
            }
            else
            {
                var resourceItem = new ResourceItem();
                resourceItem.Name = assetConfigName;
                resourceItem.Progress = 0f;
                resourceItem.IsDone = false;
                resourceItem.CallbackList.Add(process);

                if (isAppendLoading)
                {
                    _dynamicData.Add(resourceItem);
                }
                else
                {
                    _staticData = resourceItem;
                }

                while (!asyncLoad.isDone)
                {
                    resourceItem.Progress = asyncLoad.progress;
                    for (var i = 0; i < resourceItem.CallbackList.Count; i++)
                    {
                        resourceItem.CallbackList[i](false, resourceItem.Progress);
                    }

                    yield return null;
                }

                yield return new WaitForEndOfFrame();

                // loading is done
                resourceItem.IsDone = true;
                resourceItem.Progress = 1f;

                var assetBundleRequest = asyncLoad as AssetBundleRequest;
                if (assetBundleRequest != null)
                {
                    resourceItem.LinkerObject = assetBundleRequest.asset as RemoteAssetLinker;
                }
                else
                {
                    resourceItem.LinkerObject = ((ResourceRequest)asyncLoad).asset as RemoteAssetLinker;
                }

                for (var i = 0; i < resourceItem.CallbackList.Count; i++)
                {
                    resourceItem.CallbackList[i](true, asyncLoad.progress);
                }

                resourceItem.CallbackList.Clear();
            }
        }

        public static void AddAssetBundle(params AssetBundle[] assetBundleArray)
        {
            for (var i = 0; i < assetBundleArray.Length; i++)
            {
                var assetBundle = assetBundleArray[i];
                if (assetBundle == null || _assetBundles.Contains(assetBundle))
                {
                    continue;
                }

                _assetBundles.Add(assetBundle);
            }
        }

        private static ResourcesCache InitMonoBehaviour()
        {
            // create worker
            var goWorker = new GameObject("Resources worker");
            goWorker.hideFlags = HideFlags.HideInHierarchy;
            return goWorker.AddComponent<ResourcesCache>();
        }

        private static AsyncOperation AsyncLoad<TAssetLinker>(string assetName) where TAssetLinker : Object
        {
            // try load from bundles
            var bundleCount = _assetBundles.Count;
            for (var i = 0; i < bundleCount; i++)
            {
                var currentBundle = _assetBundles[i];
                var allAssetsNames = currentBundle.GetAllAssetNames();
                var targetAssetName = allAssetsNames.FirstOrDefault(s => s.EndsWith(assetName + ".asset", StringComparison.OrdinalIgnoreCase));
                if (targetAssetName != null)
                {
                    return currentBundle.LoadAssetAsync<TAssetLinker>(targetAssetName);
                }
            }

            // try load from resource
            return Resources.LoadAsync<TAssetLinker>(assetName);
        }

        private static TAssetLinker Load<TAssetLinker>(string assetName) where TAssetLinker : Object
        {
            // try load from bundles
            var bundleCount = _assetBundles.Count;
            for (var i = 0; i < bundleCount; i++)
            {
                var currentBundle = _assetBundles[i];
                var allAssetsNames = currentBundle.GetAllAssetNames();
                var targetAssetName = allAssetsNames.FirstOrDefault(s => s.EndsWith(assetName + ".asset", StringComparison.OrdinalIgnoreCase));
                if (targetAssetName != null)
                {
                    return currentBundle.LoadAsset<TAssetLinker>(targetAssetName);
                }
            }

            // try load from resource
            return Resources.Load<TAssetLinker>(assetName);
        }

        #endregion METHODS

        #region CLASS

        private class ResourceItem
        {
            #region Fields

            public readonly List<Action<bool, float>> CallbackList = new List<Action<bool, float>>();
            public bool IsDone;
            public RemoteAssetLinker LinkerObject;
            public string Name;
            public float Progress;

            #endregion Fields
        }

        #endregion CLASS
    }
}
