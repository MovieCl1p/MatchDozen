
using System;

namespace Core.Utils
{
    public class TimeUtils
    {
        private static readonly DateTime EpochStart;

        static TimeUtils()
        {
            EpochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        }

        public static int UnixTimeStamp
        {
            get
            {
                return (int)(DateTime.UtcNow - EpochStart).TotalSeconds;
            }
        }
    }
}
