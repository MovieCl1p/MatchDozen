using Core.Factories;
using UI;
using UnityEngine;

namespace Core.Controllers
{
    public abstract class SceneController : UISceneController
    {
        public SceneController()
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

//            tk2dUIManager manager = (tk2dUIManager)UnityEngine.Object.FindObjectOfType(typeof(tk2dUIManager));
//
//            if (manager == null)
//            {
//                GameObject tk2dUIManagerGameObject = new GameObject("tk2dUIManager");
//                manager = tk2dUIManagerGameObject.AddComponent<tk2dUIManager>();
//            }

//            GameObject cameraGameObject = CameraFactory.CreateInstance(CameraName.DepthMask);
//
//            manager.UICamera = cameraGameObject.GetComponent<Camera>();
//
//            manager.raycastLayerMask = (1 << cameraGameObject.layer);
//            manager.areHoverEventsTracked = false;

        }
    }
}
