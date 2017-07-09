using strange.extensions.context.impl;

namespace Core.Plugins.Strange
{
    public static class StrangeExtention
    {
        public static void Configurate(this CrossContext context, params IConfig[] configs)
        {
            for (int i = 0; i < configs.Length; i++)
            {
                context.InjectionBinder.injector.Inject(configs[i]);
                configs[i].Configure();
            }
        }
    }
}
