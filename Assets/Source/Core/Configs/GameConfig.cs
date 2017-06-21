using Configs;
using Data.Serialization;

namespace Config
{
    public class GameConfig : Config<GameConfig, JsonSerializationPolicy>
    {
        public GameConfig(string name) : base(name)
        {
        }

        public static GameConfig Default()
        {
            return new GameConfig("GameConfig");
        }
    }
}
