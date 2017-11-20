using Core.ResourceManager;
using Core.Utils.ActionUtils.Actions;

namespace Core.Commands
{
    public class LoadResourceAction : BaseAction
    {
        private readonly string _resName;
        public LoadResourceAction(string resName)
        {
            _resName = resName;
        }
        public override void Execute()
        {
            if (ResourcesCache.IsResourceLinkLoaded(_resName))
            {
                Complete();
            }
            else
            {
                ResourcesCache.SetupResourcesCache(_resName, true, OnLoadProgress);
            }
        }

        private void OnLoadProgress(bool isLoadCompleted, float progress)
        {
            if (isLoadCompleted)
            {
                Complete();
            }
        }
    }
}
