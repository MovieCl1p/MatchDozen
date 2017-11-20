using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.Editor
{
    public static class EditorHelper
    {
        [MenuItem("Assets/Create/ScriptableObject asset")]
        public static void CreateScriptableObjectAsset()
        {
            CreateScriptableObject<ScriptableObject>();
        }

        public static void CreateScriptableObject<TScriptableObject>() where TScriptableObject : ScriptableObject
        {
            var folderPath = GetSelectedObjectPath();
            var path = string.Format("{0}/{1}.asset", folderPath, typeof(TScriptableObject).Name);
            path = GetRelativePath(path, Path.GetFullPath("Assets"));
            path = AssetDatabase.GenerateUniqueAssetPath(path);

            var so = ScriptableObject.CreateInstance<TScriptableObject>();
            AssetDatabase.CreateAsset(so, path);
            AssetDatabase.Refresh();

            Selection.activeObject = so;
        }

        public static string GetSelectedObjectPath()
        {
            if (Selection.activeObject == null)
                return "Assets";

            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (string.IsNullOrEmpty(path))
                return "Assets";

            var dir = Path.GetFullPath(path);
            return dir;
        }

        public static string GetRelativePath(string absolutePath, string relativeToPath)
        {
            var uri1 = new Uri(absolutePath);
            var uri2 = new Uri(relativeToPath);
            var result = uri2.MakeRelativeUri(uri1);
            return result.ToString();
        }
    }
}
