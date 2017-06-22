using System;
using Core.Controllers;
using UI;

namespace Parachute.Loader
{
    [UISceneName("Loader")]
    public sealed class LoaderSceneController : SceneController
    {
        private bool _loaded;
        private Type _sceneToLoad;
        private object[] _args;

        public Type SceneToLoad
        {
            get
            {
                return _sceneToLoad;
            }
        }

        public LoaderSceneController()
        {
        }

        protected override void OnStart(object[] args)
        {
            _sceneToLoad = (Type)args[0];
            _args = args;

            UIManager.Load((Type)_args[0], _args);
        }
    }
}
