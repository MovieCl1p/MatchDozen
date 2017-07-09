using System;

namespace Core.Types
{
    [Flags]
    public enum PlatformFlag
    {
        All = -1,
        Web = 1,
        Android = 2,
        Apple = 4,
        Amazon = 8,
        WindowsPhone = 16,
        Mobile = 30
    }
}
