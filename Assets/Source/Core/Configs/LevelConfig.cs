using Configs;
using Data.Serialization;

namespace Config
{
    public class LevelConfig : Config<LevelConfig, JsonSerializationPolicy>
    {
        public LevelConfig(string name) : base(name)
        {
        }

        public static LevelConfig Default()
        {
            return new LevelConfig("DefaultLevel");
        }
    }
}
