namespace Assets.Scripts.Game.Model
{
    public class GridPoint
    {
        public int X { get; set; }

        public int Y { get; set; }

        public GridPoint(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            GridPoint r = (GridPoint)obj;
            if(r == null)
            {
                return false;
            }

            return r.X == this.X && r.Y == this.Y;
        }
    }
}