
namespace Assets.Scripts.Game.Model
{
    public class CellColor
    {
        public string BotColor { get; set; }

        public string TopColor { get; set; }

        public string BotMatchColor { get; set; }

        public string TopMatchColor { get; set; }

        public CellColor(string botColor, string topColor, string botMatchColor, string topMatchColor)
        {
            BotColor = botColor;
            TopColor = topColor;
            BotMatchColor = botMatchColor;
            TopMatchColor = topMatchColor;
        }
    }
}