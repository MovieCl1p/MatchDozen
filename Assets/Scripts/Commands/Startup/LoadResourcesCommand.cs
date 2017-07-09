using AssetControl;
using Core.Commands;
using Core.Utils.ActionUtils;
using Core.Utils.ActionUtils.Actions;
using GuiFactory;
using strange.extensions.command.impl;

namespace Commands.Startup
{
    public class LoadResourcesCommand : Command
    {
        public override void Execute()
        {
            Retain();
            var loadQueue = new ActionQueue();
            loadQueue.AddAction(new LoadResourceAction(AssetsNames.UiConfigsAssetsLinks));
            loadQueue.AddAction(new LoadResourceAction(AssetsNames.UiAssetsLinks));
            loadQueue.AddAction(new LoadResourceAction(AssetsNames.AudioAssetsLinks));
            loadQueue.AddAction(new LoadResourceAction(AssetsNames.AudioMapAssetsLinks));
            loadQueue.AddAction(new InvokeMethodAction(OnLoadAssetsComplete));
            loadQueue.Start();
        }

        private void OnLoadAssetsComplete()
        {
            GuiConfigFactory.Init();
            Release();
        }
    }
}
