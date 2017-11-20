using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;
using System.IO;
using System.Linq;
using System;
using System.Collections.Generic;

namespace Core.ResourceManager.RALExtension.Editor
{
    [CustomEditor(typeof(RemoteAssetLinker))]
    public class RemoteAssetLinkerEditor : UnityEditor.Editor
    {
        private const string LinkerRootFolder = "/RemoteAssetLinker/";
        private const string BindedScenesPath = "Assets/Scene/AssetLinkerBind/";
        // path for place remote asset linker
        private const string LinkerPath = "Assets/Resources/RemoteAssetLinker.asset";

        /// <summary>
        /// Handle search from top menu
        /// </summary>
        [MenuItem("Plarium/Update asset links")]
        public static void UpdateAssetLinks()
        {
            // get current linker
            var allRemoteAssetLinker = Resources.LoadAll<RemoteAssetLinker>("");
            if (allRemoteAssetLinker == null || allRemoteAssetLinker.Length == 0)
            {
                var remoteAssetLinker = CreateInstance<RemoteAssetLinker>();
                AssetDatabase.CreateAsset(remoteAssetLinker, LinkerPath);
                AssetDatabase.SaveAssets();
                allRemoteAssetLinker = new[] { remoteAssetLinker };
            }

            for (int i = 0; i < allRemoteAssetLinker.Length; i++)
            {
                var remoteAssetLinker = allRemoteAssetLinker[i];

                // reset linker
                ResetItemList(remoteAssetLinker);
                EditorUtility.SetDirty(remoteAssetLinker);
                AssetDatabase.SaveAssets();
            }
        }

        public static void ExcludeAssetLinkersFromResourceToTemp()
        {
            var destinationForTemp = Application.temporaryCachePath + "/TempResource/";
            if (!Directory.Exists(destinationForTemp))
            {
                Directory.CreateDirectory(destinationForTemp);
            }

            foreach (var guid in AssetDatabase.FindAssets("t:RemoteAssetLinker"))
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(RemoteAssetLinker)) as RemoteAssetLinker;
                if (asset == null)
                {
                    continue;
                }

                if (!asset.IsTemporaryIgnore && !asset.IsUseAsAssetBundle)
                {
                    continue;
                }

                var tempFileDestination = destinationForTemp + assetPath;
                var tempFolder = Path.GetDirectoryName(tempFileDestination);
                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }

                if (File.Exists(tempFileDestination))
                {
                    File.Delete(tempFileDestination);
                }

                File.Move(assetPath, tempFileDestination);
            }

            AssetDatabase.Refresh();
        }

        public static void ReturnAssetLinkersToResources()
        {
            var destinationForTemp = Application.temporaryCachePath + "/TempResource/Assets";
            if (!Directory.Exists(destinationForTemp))
            {
                return;
            }

            var directoryInfo = new DirectoryInfo(destinationForTemp);
            var allFiles = directoryInfo.GetFiles("*.*", SearchOption.AllDirectories);
            for (var i = 0; i < allFiles.Length; i++)
            {
                var currentFileInfo = allFiles[i];
                var oldSourcePath = currentFileInfo.FullName.Replace(directoryInfo.FullName, Application.dataPath);
                if (!File.Exists(oldSourcePath))
                {
                    currentFileInfo.MoveTo(oldSourcePath);
                }
            }

            AssetDatabase.Refresh();
        }

        public static void RemoveBindedScene(RemoteAssetLinker remoteAssetLinker)
        {
            var assetPath = AssetDatabase.GetAssetPath(remoteAssetLinker);
            var rootIndex = assetPath.LastIndexOf(LinkerRootFolder, StringComparison.Ordinal) + LinkerRootFolder.Length;

            var directoryName = Path.GetDirectoryName(assetPath.Substring(rootIndex, assetPath.Length - rootIndex));

            // save
            var path = BindedScenesPath + directoryName + "/" + remoteAssetLinker.name + ".unity";

            // add to build list
            var original = EditorBuildSettings.scenes.ToList();
            var sceneInBuildSettings = original.FirstOrDefault(settingsScene => settingsScene.path == path);
            if (sceneInBuildSettings != null)
            {
                original.Remove(sceneInBuildSettings);

                //var newSettings = new EditorBuildSettingsScene[original.Length + 1];
                //Array.Copy(original, newSettings, original.Length);
                //var sceneToAdd = new EditorBuildSettingsScene(path, true);
                //newSettings[newSettings.Length - 1] = sceneToAdd;
                EditorBuildSettings.scenes = original.ToArray();
            }

            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Inspector type
        /// </summary>
        public override void OnInspectorGUI()
        {
            RemoteAssetLinker linker = target as RemoteAssetLinker;
            if (linker == null)
            {
                return;
            }

            var enabled = GUI.enabled;

            GUI.enabled = !linker.IsExternal;

            // bottom line
            DrawCustomSettings(linker);
            DrawControlLine(linker, true);
            CustomGui.Splitter(Color.gray, 2);

            // iteration by items
            for (int i = 0; i < linker.CollectionLinks.Count; i++)
            {
                var currentItem = linker.CollectionLinks[i];

                var rect = EditorGUILayout.BeginHorizontal();
                {
                    float buttonWidth = 20f;

                    var buttonRect = new Rect();
                    buttonRect.x = rect.x + rect.width - buttonWidth * 2f;
                    buttonRect.y = rect.y;
                    buttonRect.width = buttonWidth;
                    buttonRect.height = buttonWidth;

                    buttonRect.x = rect.x + rect.width - buttonWidth;

                    GUI.backgroundColor = Color.red;
                    if (GUI.Button(buttonRect, "-"))
                    {
                        linker.CollectionLinks.Remove(currentItem);
                        Dirty(linker);
                    }

                    GUI.backgroundColor = Color.white;

                    currentItem.IsExpanded = EditorGUILayout.Foldout(currentItem.IsExpanded, currentItem.Path);
                }
                EditorGUILayout.EndHorizontal();

                GUI.backgroundColor = Color.white;

                if (currentItem.IsExpanded)
                {
                    // path line
                    if (DrawPathLine(i, currentItem, linker))
                    {
                        break;
                    }

                    DrawClassTarget(i, currentItem, linker);    // draw class line
                    DrawFilterLine(i, currentItem, linker);     // filter line
                    DrawItems(currentItem);             // objects line
                    DrawSubItems(currentItem);          // sub objects line
                }

                GUI.contentColor = Color.white;

                CustomGui.Splitter(Color.gray, 2);
            }

            // bottom line
            DrawControlLine(linker, false);

            GUI.enabled = enabled;
        }

        /// <summary>
        /// Search in current remote asset linker
        /// </summary>
        /// <param name="linker">ScriptableObject</param>
        public static void ResetItemList(RemoteAssetLinker linker)
        {
            for (int i = 0; i < linker.CollectionLinks.Count; i++)
            {
                var linkItem = linker.CollectionLinks[i];

                ResetItem(linkItem, linker);
            }
        }

        /// <summary>
        /// Search in single link key
        /// </summary>
        /// <param name="linkItem">Single linker item</param>
        /// <param name="linker"></param>
        private static void ResetItem(RemoteAssetLinker.LinkKey linkItem, RemoteAssetLinker linker)
        {
            if (!string.IsNullOrEmpty(linkItem.Path))
            {
                TypeNameToType(linkItem);

                // search by all recursive assets
                var guids = AssetDatabase.FindAssets(linkItem.Filter, new[] { linkItem.Path });
                guids = RemoveDuplicates(guids);
                linkItem.Items.Clear();
                linkItem.SubLinkKeys.Clear();

                for (var j = 0; j < guids.Length; j++)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[j]);
                    var normalizedPath = NormalizePath(path);

                    // if recursive item
                    if (normalizedPath != NormalizePath(linkItem.Path))
                    {
                        // is ignore sub assets
                        if (!linkItem.IsRecursive)
                        {
                            continue;
                        }

                        // add subLinkKey
                        var target = linkItem.SubLinkKeys.FirstOrDefault(key => NormalizePath(key.Path) == normalizedPath);
                        if (target == null)
                        {
                            target = new RemoteAssetLinker.SubLinkKey
                            {
                                Path = normalizedPath,
                                Section = Path.GetFileName(normalizedPath)
                            };

                            linkItem.SubLinkKeys.Add(target);
                        }

                        // check type
                        var subObj = GetAsset(linkItem, path);
                        if (subObj == null)
                        {
                            continue;
                        }

                        target.Items.Add(subObj);
                        continue;
                    }

                    var obj = GetAsset(linkItem, path);
                    if (obj == null)
                    {
                        continue;
                    }

                    linkItem.Items.Add(obj);
                }

                var linkerPath = AssetDatabase.GetAssetPath(linker);
                var dir = Path.GetDirectoryName(linkerPath);
                var fileName = Path.GetFileNameWithoutExtension(linkerPath);
                var newPath = dir + "/" + fileName;

                if (linkItem.IsExternal)
                {
                    if (!AssetDatabase.IsValidFolder(newPath))
                    {
                        AssetDatabase.CreateFolder(dir, fileName);
                    }

                    var linkGroup = linkItem.SubLinkKeys.GroupBy(key => key.Section)
                        .ToDictionary(si => si.Key, si => si.ToList());

                    var remoteLinks = linkItem.RemoteLinks;
                    var keys = remoteLinks.Keys.ToList();

                    // Remove defunct assets.
                    for (int i = keys.Count - 1; i >= 0; i--)
                    {
                        var key = keys[i];

                        if (!linkGroup.ContainsKey(key))
                        {
                            linkItem.ExternalPasses.Remove(remoteLinks[key]);
                            remoteLinks.Remove(key);

                            var assetPath = newPath + "/" + key + ".asset";

                            CheckExternalLinker(linkItem, assetPath, null);
                        }
                    }

                    foreach (var pair in linkGroup)
                    {
                        var tempPair = pair;

                        var assetPath = newPath + "/" + tempPair.Key + ".asset";

                        CheckExternalLinker(linkItem, assetPath, tempPair.Value);

                        if (!linkItem.ExternalPasses.Contains(assetPath))
                        {
                            linkItem.ExternalPasses.Add(assetPath);
                        }
                    }

                    linkItem.SubLinkKeys = new List<RemoteAssetLinker.SubLinkKey>();
                }
                else
                {
                    var group = linkItem.SubLinkKeys.GroupBy(key => key.Section).ToDictionary(si => si.Key, si => si.ToList());

                    foreach (var pair in @group)
                    {
                        var sectionName = pair.Key;
                        var assetPath = newPath + "/" + sectionName + ".asset";

                        var existsAsset = AssetDatabase.LoadAssetAtPath<RemoteAssetLinker>(assetPath);

                        if (existsAsset != null)
                        {
                            var existsBlock = existsAsset.CollectionLinks.FirstOrDefault(x => x.Section == linkItem.Section);

                            if (existsBlock != null)
                            {
                                existsAsset.CollectionLinks.Remove(existsBlock);
                            }

                            if (existsAsset.CollectionLinks.Count == 0)
                            {
                                RemoveBindedScene(existsAsset);
                                AssetDatabase.DeleteAsset(assetPath);
                            }
                        }
                    }
                }
            }
        }

        private static void CheckExternalLinker(RemoteAssetLinker.LinkKey linkKey, string assetPath, List<RemoteAssetLinker.SubLinkKey> links)
        {
            var existsAsset = AssetDatabase.LoadAssetAtPath<RemoteAssetLinker>(assetPath);

            if (existsAsset == null)
            {
                existsAsset = CreateInstance<RemoteAssetLinker>();
                existsAsset.IsExternal = true;

                AssetDatabase.CreateAsset(existsAsset, assetPath);
            }

            var existsLinkKey = existsAsset.CollectionLinks.FirstOrDefault(x => x.Section == linkKey.Section);

            if (links == null && existsLinkKey != null)
            {
                existsAsset.CollectionLinks.Remove(existsLinkKey);
            }
            else if (existsLinkKey != null)
            {
                existsLinkKey.Section = linkKey.Section;
                existsLinkKey.Path = linkKey.Path;
                existsLinkKey.Filter = linkKey.Filter;
                existsLinkKey.IsRecursive = linkKey.IsRecursive;
                existsLinkKey.TypeName = linkKey.TypeName;
                existsLinkKey.SubLinkKeys = links;
                existsLinkKey.IsRemote = true;
                existsLinkKey.IsExpanded = true;
            }
            else
            {
                var newLinkKey = new RemoteAssetLinker.LinkKey();

                newLinkKey.Section = linkKey.Section;
                newLinkKey.Path = linkKey.Path;
                newLinkKey.Filter = linkKey.Filter;
                newLinkKey.IsRecursive = linkKey.IsRecursive;
                newLinkKey.TypeName = linkKey.TypeName;
                newLinkKey.SubLinkKeys = links;
                newLinkKey.IsRemote = true;
                newLinkKey.IsExpanded = true;

                existsAsset.CollectionLinks.Add(newLinkKey);
            }

            if (existsAsset.CollectionLinks.Count == 0)
            {
                RemoveBindedScene(existsAsset);
                AssetDatabase.DeleteAsset(assetPath);
            }
            else
            {
                EditorUtility.SetDirty(existsAsset);
            }
        }

        private static string[] RemoveDuplicates(string[] s)
        {
            HashSet<string> set = new HashSet<string>(s);
            string[] result = new string[set.Count];
            set.CopyTo(result);
            return result;
        }

        /// <summary>
        /// Get asset by type
        /// </summary>
        /// <param name="linkItem">Linker item</param>
        /// <param name="path">Directory</param>
        /// <returns>Response object</returns>
        private static Object GetAsset(RemoteAssetLinker.LinkKey linkItem, string path)
        {
            Object subObj;
            if (linkItem.TargetType != null)
            {
                subObj = AssetDatabase.LoadAssetAtPath(path, linkItem.TargetType);
                if (subObj is DefaultAsset)
                {
                    return null;
                }
            }
            else
            {
                subObj = AssetDatabase.LoadAssetAtPath(path, typeof(Object));
                if (subObj is DefaultAsset)
                {
                    return null;
                }
            }

            return subObj;
        }

        /// <summary>
        /// Convert string type to Type
        /// </summary>
        /// <param name="linkItem">Linker item</param>
        private static void TypeNameToType(RemoteAssetLinker.LinkKey linkItem)
        {
            if (string.IsNullOrEmpty(linkItem.TypeName))
            {
                linkItem.TargetType = null;
                return;
            }

            // search type
            linkItem.TargetType = Type.GetType(linkItem.TypeName);
            if (linkItem.TargetType != null)
            {
                return;
            }

            // search other
            foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            {
                linkItem.TargetType = a.GetType(linkItem.TypeName);
                if (linkItem.TargetType != null)
                {
                    return;
                }
            }

            linkItem.TargetType = null;
        }

        /// <summary>
        /// Normalize path
        /// </summary>
        /// <param name="path">Custom path</param>
        /// <returns>Return standart path</returns>
        private static string NormalizePath(string path)
        {
            var pathVar = path;
            if (!string.IsNullOrEmpty(Path.GetExtension(pathVar)))
            {
                pathVar = Path.GetDirectoryName(path);
            }

            return pathVar == null ? path : pathVar.Replace("\\", "/");
        }

        /// <summary>
        /// Draw path line
        /// </summary>
        /// <param name="index">Index of linker item</param>
        /// <param name="currentItem">Linker item</param>
        /// <param name="linker">Linker</param>
        /// <returns>Return true if removed</returns>
        private static bool DrawPathLine(int index, RemoteAssetLinker.LinkKey currentItem, RemoteAssetLinker linker)
        {
            GUILayout.BeginHorizontal();

            // path
            var itemId = "Path" + index;
            GUILayout.Label("Path: ", GUILayout.Width(40));
            var visualPath = currentItem.Path;

            GUI.SetNextControlName(itemId);
            var maxWidth = EditorGUIUtility.currentViewWidth - 80;
            currentItem.Path = GUILayout.TextField(currentItem.Path, GUILayout.MaxWidth(maxWidth));
            currentItem.Path = CopyPast(currentItem.Path, itemId);
            //if (currentItem.Path != visualPath)
            //{
            //	Dirty(linker);
            //}

//            if (currentItem.Path.KeyPressed(itemId, KeyCode.Return, out visualPath))
//            {
//                Dirty(linker);
//            }

            GUILayout.EndHorizontal();
            return false;
        }

        /// <summary>
        /// Draw class target
        /// </summary>
        /// <param name="index">Index of linker item</param>
        /// <param name="currentItem">Linker item</param>
        private static void DrawClassTarget(int index, RemoteAssetLinker.LinkKey currentItem, RemoteAssetLinker linker)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Class target: ", GUILayout.Width(80));
            var visualClassName = currentItem.TypeName;

            var itemId = "Class" + index;
            GUI.SetNextControlName(itemId);
            currentItem.TypeName = GUILayout.TextField(currentItem.TypeName);
            currentItem.TypeName = CopyPast(currentItem.TypeName, itemId);
            //if (currentItem.TypeName != visualClassName)
            //{
            //	Dirty(linker);
            //}

//            if (currentItem.TypeName.KeyPressed(itemId, KeyCode.Return, out visualClassName))
//            {
//                Dirty(linker);
//            }

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw filter line
        /// </summary>
        /// <param name="index">Index of linker item</param>
        /// <param name="currentItem">Linker item</param>
        private static void DrawFilterLine(int index, RemoteAssetLinker.LinkKey currentItem, RemoteAssetLinker linker)
        {
            EditorGUILayout.BeginHorizontal();

            // filter name
            var itemId = "Filter" + index;
            EditorGUILayout.LabelField("Filter: ", GUILayout.Width(40));
            var visualFilter = currentItem.Filter;

            GUI.SetNextControlName(itemId);
            var maxWidth = EditorGUIUtility.currentViewWidth - 180;
            currentItem.Filter = EditorGUILayout.TextField(currentItem.Filter, GUILayout.MaxWidth(maxWidth * 0.6f));
            currentItem.Filter = CopyPast(currentItem.Filter, itemId);
            //if (currentItem.Filter != visualFilter)
            //{
            //	Dirty(linker);
            //}

//            if (currentItem.Filter.KeyPressed(itemId, KeyCode.Return, out visualFilter))
//            {
//                Dirty(linker);
//            }

            // section name
            var sectionId = "Section" + index;
            EditorGUILayout.LabelField("Section: ", GUILayout.Width(50));
            var visualSection = currentItem.Section;

            GUI.SetNextControlName(sectionId);
            currentItem.Section = EditorGUILayout.TextField(currentItem.Section, GUILayout.MaxWidth(maxWidth * 0.4f));
            currentItem.Section = CopyPast(currentItem.Section, sectionId);

            //if (currentItem.Section != visualSection)
            //{
            //	Dirty(linker);
            //}

//            if (currentItem.Section.KeyPressed(sectionId, KeyCode.Return, out visualSection))
//            {
//                Dirty(linker);
//            }

            EditorGUILayout.BeginVertical();
            {
                var size = GUIStyle.none.CalcSize(new GUIContent("Is recursive"));

                // recursive
                EditorGUILayout.BeginHorizontal();
                {
                    var newRecursiveValue = EditorGUILayout.Toggle(currentItem.IsRecursive, GUILayout.Width(17f));
                    EditorGUILayout.LabelField("Is recursive", GUILayout.Width(size.x + 5f));

                    if (currentItem.IsRecursive != newRecursiveValue)
                    {
                        currentItem.IsRecursive = newRecursiveValue;
                        Dirty(linker);
                    }
                }
                EditorGUILayout.EndHorizontal();

                // remote
                EditorGUILayout.BeginHorizontal();
                {
                    var newExternalValue = EditorGUILayout.Toggle(currentItem.IsExternal, GUILayout.Width(17f));
                    EditorGUILayout.LabelField("Is external", GUILayout.Width(size.x + 5f));

                    if (currentItem.IsExternal != newExternalValue)
                    {
                        currentItem.IsExternal = newExternalValue;
                        Dirty(linker);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw items
        /// </summary>
        /// <param name="currentItem">Linker item</param>
        private static void DrawItems(RemoteAssetLinker.LinkKey currentItem)
        {
            GUI.contentColor = Color.green;
            for (int j = 0; j < currentItem.Items.Count; j++)
            {
                var item = currentItem.Items[j];
                EditorGUILayout.LabelField("   Item: " + item);
            }
        }

        /// <summary>
        /// Draw sub items
        /// </summary>
        /// <param name="currentItem">Linker item</param>
        private static void DrawSubItems(RemoteAssetLinker.LinkKey currentItem)
        {
            GUI.contentColor = Color.yellow;
            var group = currentItem.SubLinkKeys.GroupBy(key => key.Section).ToDictionary(si => si.Key, si => si.ToList());
            foreach (var pair in @group)
            {
                EditorGUILayout.LabelField("      Section: " + pair.Key);

                for (int j = 0; j < pair.Value.Count; j++)
                {
                    var subItem = pair.Value[j];
                    for (int k = 0; k < subItem.Items.Count; k++)
                    {
                        var label = "         SubItem: " + subItem.Items[k];
                        EditorGUILayout.LabelField(label);
                    }
                }
            }
        }

        private static void DrawCustomSettings(RemoteAssetLinker linker)
        {
            GUILayout.BeginHorizontal();

            var isAssetBundle = linker.IsUseAsAssetBundle;
            var newIsAssetBundle = GUILayout.Toggle(isAssetBundle, "Is Asset Bundle");
            if (newIsAssetBundle != isAssetBundle)
            {
                linker.IsUseAsAssetBundle = newIsAssetBundle;
                Dirty(linker);
            }

            var isForDevelopment = linker.IsForDevelopment;
            var newIsForDevelopment = GUILayout.Toggle(isForDevelopment, "Is for Development");
            if (newIsForDevelopment != isForDevelopment)
            {
                linker.IsForDevelopment = newIsForDevelopment;
                Dirty(linker);
            }

            var isTemporaryIgnore = linker.IsTemporaryIgnore;
            var newIsTemporaryIgnore = GUILayout.Toggle(isTemporaryIgnore, "Is Ignore");
            if (newIsTemporaryIgnore != isTemporaryIgnore)
            {
                linker.IsTemporaryIgnore = newIsTemporaryIgnore;
                Dirty(linker);
            }

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Draw control line
        /// </summary>
        /// <param name="linker">Linker</param>
        private static void DrawControlLine(RemoteAssetLinker linker, bool isTop)
        {
            GUILayout.BeginHorizontal();

            // add new elements
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("+"))
            {
                var newLinkKey = new RemoteAssetLinker.LinkKey();
                if (isTop && linker.CollectionLinks.Count > 0)
                {
                    linker.CollectionLinks.Insert(0, newLinkKey);
                }
                else
                {
                    linker.CollectionLinks.Add(newLinkKey);
                }

                Dirty(linker);
            }

            // Reset
            GUI.backgroundColor = Color.blue;
            if (GUILayout.Button("Reset"))
            {
                Dirty(linker);
            }

            GUI.backgroundColor = Color.white;
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Set to update linker
        /// </summary>
        private static void Dirty(RemoteAssetLinker linker)
        {
            // reset linker
            ResetItemList(linker);
            EditorUtility.SetDirty(linker);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Copy/Paste tools
        /// </summary>
        /// <param name="value">Source value string</param>
        /// <param name="itemId">UI ID</param>
        /// <returns>Return result value string</returns>
        private static string CopyPast(string value, string itemId)
        {
            Event e = Event.current;
            if (GUI.GetNameOfFocusedControl() == itemId && e.type == EventType.KeyUp)
            {
                // paste
                if (e.control && e.keyCode == KeyCode.V)
                {
                    TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                    if (string.IsNullOrEmpty(editor.SelectedText))
                    {
                        return value.Insert(editor.cursorIndex, GUIUtility.systemCopyBuffer);
                    }

                    return value.Replace(editor.SelectedText, GUIUtility.systemCopyBuffer);
                }

                // copy
                if (e.control && e.keyCode == KeyCode.C)
                {
                    TextEditor editor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                    GUIUtility.systemCopyBuffer = editor.SelectedText;
                }

                return value;
            }

            return value;
        }
    }
}
