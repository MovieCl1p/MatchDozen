using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.ResourceManager.RALExtension
{
    public class RemoteAssetLinker : ScriptableObject
    {
        #region Fields

        /// <summary>
        /// List of link to resources
        /// </summary>
        [SerializeField]
        public List<LinkKey> CollectionLinks = new List<LinkKey>();

        /// <summary>
        /// Create sub asset linkers
        /// </summary>
        [SerializeField]
        public bool IsExternal;

        /// <summary>
        /// This asset linker used as asset bundle
        /// </summary>
        [SerializeField]
        public bool IsUseAsAssetBundle;

        /// <summary>
        /// This asset linker used only for development
        /// </summary>
        [SerializeField]
        public bool IsForDevelopment;

        /// <summary>
        /// This asset linker exclude from all process
        /// </summary>
        [SerializeField]
        public bool IsTemporaryIgnore;

        #endregion Fields

        #region Methods

        /// <summary>
        /// Get all default object from collection
        /// </summary>
        /// <param name="sectionName">Target section name</param>
        /// <returns>Return result array</returns>
        public Object[] GetObject(string sectionName)
        {
            return GetAllObject<Object>(sectionName);
        }

        /// <summary>
        /// Get default object from collection
        /// </summary>
        /// <param name="sectionName">Target section name</param>
        /// <param name="namePrefab">Target prefab name</param>
        /// <returns>Return result array</returns>
        public Object GetObject(string sectionName, string namePrefab)
        {
            return GetObject<Object>(sectionName, namePrefab);
        }

        /// <summary>
        /// Get all object from collection by type
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="sectionName">Target section name</param>
        /// <returns>Return result array</returns>
        public T[] GetAllObject<T>(string sectionName) where T : Object
        {
            List<T> result = null;
            for (var i = 0; i < CollectionLinks.Count; i++)
            {
                var currentLinkKey = CollectionLinks[i];

                // direct search
                var isDirectSearchDone = currentLinkKey.Section == sectionName;
                if (isDirectSearchDone)
                {
                    AddToResult(ref result, currentLinkKey.Items);
                }

                // sub search
                if (currentLinkKey.IsRecursive)
                {
                    for (int j = 0; j < currentLinkKey.SubLinkKeys.Count; j++)
                    {
                        var subLink = currentLinkKey.SubLinkKeys[j];
                        if (isDirectSearchDone || subLink.Section == sectionName)
                        {
                            AddToResult(ref result, subLink.Items);
                        }
                    }
                }
            }

            return result != null ? result.ToArray() : null;
        }

        /// <summary>
        /// Get object from collection by type
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="sectionName">Target section name</param>
        /// <param name="namePrefab">Target prefab name</param>
        /// <returns>Return single result</returns>
        public T GetObject<T>(string sectionName, string namePrefab) where T : Object
        {
            for (var i = 0; i < CollectionLinks.Count; i++)
            {
                var currentLinkKey = CollectionLinks[i];

                // direct search
                var isDirectSearchDone = currentLinkKey.Section == sectionName;
                if (isDirectSearchDone)
                {
                    var targetObject = currentLinkKey.GetObjectByName(namePrefab) as T;
                    if (targetObject != null)
                    {
                        return targetObject;
                    }
                }

                // sub search
                if (currentLinkKey.IsRecursive)
                {
                    for (int j = 0; j < currentLinkKey.SubLinkKeys.Count; j++)
                    {
                        var subLink = currentLinkKey.SubLinkKeys[j];
                        if (isDirectSearchDone || subLink.Section == sectionName)
                        {
                            var targetObject = subLink.GetObjectByName(namePrefab) as T;
                            if (targetObject != null)
                            {
                                return targetObject;
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Collecting to result
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="result">Result of search</param>
        /// <param name="rangeTargets">Range results</param>
        /// <param name="singleTarget">Single result</param>
        private void AddToResult<T>(ref List<T> result, IList<Object> rangeTargets, Object singleTarget = null) where T : Object
        {
            if (singleTarget == null)
            {
                for (int i = 0; i < rangeTargets.Count; i++)
                {
                    var obj = rangeTargets[i];
                    if (obj is T)
                    {
                        if (result == null)
                        {
                            result = new List<T>();
                        }

                        result.Add(obj as T);
                    }
                }
            }
            else if (singleTarget is T)
            {
                if (result == null)
                {
                    result = new List<T>();
                }

                result.Add(singleTarget as T);
            }
        }

        #endregion Methods

        #region Classes

        [DebuggerDisplay("LinkKey Path={Path} Section={Section} IsRecursive={IsRecursive} IsExternal={IsExternal}")]
        [Serializable]
        public class LinkKey
        {
            #region Fields

            public List<string> ExternalPasses = new List<string>();
            public string Filter = string.Empty;
            public bool IsExpanded;
            public bool IsExternal;
            public bool IsRecursive;
            public bool IsRemote;
            public List<Object> Items = new List<Object>();
            public string Path = string.Empty;
            public string Section = string.Empty;
            public List<SubLinkKey> SubLinkKeys = new List<SubLinkKey>();
            public Type TargetType;
            public string TypeName = string.Empty;

            private Dictionary<string, Object> _cacheDictionary = new Dictionary<string, Object>();
            private readonly Dictionary<string, string> _remoteLinks = new Dictionary<string, string>();
            private readonly HashSet<string> _requestSet = new HashSet<string>();

            #endregion Fields

            #region Properties

            public Dictionary<string, string> RemoteLinks
            {
                get
                {
                    _remoteLinks.Clear();

                    for (int i = 0; i < ExternalPasses.Count; i++)
                    {
                        var linkerPath = ExternalPasses[i];
                        var linkerSection = System.IO.Path.GetFileNameWithoutExtension(linkerPath.TrimEnd(System.IO.Path.DirectorySeparatorChar));

                        _remoteLinks[linkerSection] = linkerPath;
                    }

                    return _remoteLinks;
                }
            }

            #endregion Properties

            #region Methods

            public Object GetObjectByName(string objectName)
            {
                Object result;

                // try load from cache
                if (_cacheDictionary.TryGetValue(objectName, out result))
                {
                    return result;
                }

                // is name requested
                if (_requestSet.Contains(objectName))
                {
                    return null;
                }

                // new search
                _requestSet.Add(objectName);
                var itemsCount = Items.Count;
                for (var i = 0; i < itemsCount; i++)
                {
                    var item = Items[i];

                    // check "empty" objects
                    if (item == null || item.ToString().Equals("null") || string.IsNullOrEmpty(item.name))
                    {
                        continue;
                    }

                    // search
                    var assetPath = Path + "/" + item.name;
                    if (!assetPath.EndsWith("/" + objectName, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    // return result
                    _cacheDictionary.Add(objectName, item);
                    return item;
                }

                // result not found
                return null;
            }

            #endregion Methods
        }

        [DebuggerDisplay("SubLinkKey Path={Path} Section={Section}")]
        [Serializable]
        public class SubLinkKey
        {
            #region Fields

            public List<Object> Items = new List<Object>();
            public string Path = string.Empty;
            public string Section = string.Empty;

            private readonly Dictionary<string, Object> _cacheDictionary = new Dictionary<string, Object>();
            private readonly HashSet<string> _requestSet = new HashSet<string>();

            #endregion Fields

            #region Methods

            public Object GetObjectByName(string objectName)
            {
                Object result;

                // try load from cache
                if (_cacheDictionary.TryGetValue(objectName, out result))
                {
                    return result;
                }

                // is name requested
                if (_requestSet.Contains(objectName))
                {
                    return null;
                }

                // new search
                _requestSet.Add(objectName);
                var itemsCount = Items.Count;
                for (var i = 0; i < itemsCount; i++)
                {
                    var item = Items[i];

                    // check "empty" objects
                    if (item == null || item.ToString().Equals("null") || string.IsNullOrEmpty(item.name))
                    {
                        continue;
                    }

                    // search
                    var assetPath = Path + "/" + item.name;
                    if (!assetPath.EndsWith("/" + objectName, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    // return result
                    _cacheDictionary.Add(objectName, item);
                    return item;
                }

                // result not found
                return null;
            }

            #endregion Methods
        }

        #endregion Classes
    }
}
