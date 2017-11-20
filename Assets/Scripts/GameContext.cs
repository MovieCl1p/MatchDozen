using strange.extensions.command.api;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using Signals;
using UnityEngine;

public class GameContext : MVCSContext
{
    public GameContext(MonoBehaviour view) : base(view)
    {
    }

    public GameContext(MonoBehaviour view, ContextStartupFlags flags) : base(view, flags)
    {

    }

    protected override void addCoreComponents()
    {
        base.addCoreComponents();
        InjectionBinder.Unbind<ICommandBinder>();
        InjectionBinder.Bind<ICommandBinder>().To<SignalCommandBinder>().ToSingleton();
    }

    public override void Launch()
    {
        StartSignal startSignal = InjectionBinder.GetInstance<StartSignal>();
        startSignal.Dispatch();
    }

    public void OnApplicationClose()
    {

    }
}