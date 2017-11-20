using System.IO;
using System.Net;

namespace Core.Utils
{
    public class FileUtils
    {
        public static string LoadFile(string url, string contentType, int timeout = 30000)
        {
            ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, errors) => true;

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = timeout;
            request.ContentType = contentType;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var result = response.GetResponseStream();
                if (result != null)
                {
                    using (var reader = new StreamReader(result))
                    {
                        string responseString = reader.ReadToEnd();
                        if (!string.IsNullOrEmpty(responseString))
                        {
                            return responseString;
                        }
                    }
                }
            }

            return "";
        }

        public static string ReadFile(DirectoryInfo directory, string fileName)
        {
            FileInfo[] fileInfo = directory.GetFiles(fileName);

            if (fileInfo.Length == 0)
                return null;

            string filePath = fileInfo[0].FullName;

            return File.ReadAllText(filePath);
        }

        private static string _persistentDataPath;

        public static string GetPersistandPath()
        {
            if (_persistentDataPath != null)
            {
                return _persistentDataPath;
            }

            string path = "";

#if UNITY_EDITOR
            if (!Directory.Exists("Assets/LocalData"))
            {
                Directory.CreateDirectory("Assets/LocalData");
            }

            path = @"Assets/LocalData/";

#elif UNITY_ANDROID
            try
            {
                System.IntPtr obj_context = AndroidJNI.FindClass("android/content/ContextWrapper");
                System.IntPtr method_getFilesDir = AndroidJNIHelper.GetMethodID(obj_context, "getFilesDir", "()Ljava/io/File;");

                using (AndroidJavaClass cls_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    using (AndroidJavaObject obj_Activity = cls_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                    {
                        System.IntPtr file = AndroidJNI.CallObjectMethod(obj_Activity.GetRawObject(), method_getFilesDir, new jvalue[0]);
                        System.IntPtr obj_file = AndroidJNI.FindClass("java/io/File");
                        System.IntPtr method_getAbsolutePath = AndroidJNIHelper.GetMethodID(obj_file, "getAbsolutePath", "()Ljava/lang/String;");

                        path = AndroidJNI.CallStringMethod(file, method_getAbsolutePath, new jvalue[0]);

                        if (path == null)
                        {
                            Debug.Log("Using fallback path");
                            path = string.Format("/data/data/{0}/files", Application.bundleIdentifier);
                        }
                    }
                }
             }
            catch(System.Exception e)
            {
                Debug.Log(e.ToString());
            }
#else
            path = Application.persistentDataPath;
#endif

            return _persistentDataPath = path;
        }
    }
}
