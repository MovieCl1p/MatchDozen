using Configs;
using Core.Plugins.Strange;
using strange.extensions.context.api;
using strange.extensions.context.impl;

public class Root : ContextView
{
    protected void Awake()
    {
        context = new GameContext(this, ContextStartupFlags.MANUAL_LAUNCH);
        var crossContext = (CrossContext)context;

        crossContext.Configurate(new GameConfig());

        context.Launch();
    }

    protected void OnApplicationQuit()
    {
        if (context == null)
        {
            return;
        }

        ((GameContext)context).OnApplicationClose();
    }

}