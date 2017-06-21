using System.Collections.Generic;
using UnityEngine;

namespace Core.Factories
{
    public enum CameraName
    {
        DepthMask
    }

    public static class CameraFactory
    {
        private static Dictionary<CameraName, string> _cameraToPrefab;

        static CameraFactory()
        {
            _cameraToPrefab = new Dictionary<CameraName, string>();

            _cameraToPrefab.Add(CameraName.DepthMask, "Prefabs/Camera/depthMaskCamera");
        }

        public static GameObject CreateInstance(CameraName cameraName)
        {

            GameObject gameObject = (GameObject)GameObject.Instantiate(
                Resources.Load(_cameraToPrefab[cameraName]));

            gameObject.name = cameraName.ToString();

            return gameObject;
        }

    }
}
