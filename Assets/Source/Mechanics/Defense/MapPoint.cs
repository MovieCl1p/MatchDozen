using System;
using UnityEngine;
using System.Runtime.Serialization;

namespace Mechanics.Defense
{
	[Serializable]
    public sealed class MapPoint : ISerializable
    {
		#region ISerializable implementation

		public void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			info.AddValue("x",x);
			info.AddValue("y",y);
		}

		public MapPoint(SerializationInfo info, StreamingContext context)
		{
			x = info.GetInt32("x");
			y = info.GetInt32("y");
		}

		#endregion

        public int x { get; private set; }
        public int y { get; private set; }

        public MapPoint()
        {
        }

        public MapPoint(int iX, int iY)
        {
            this.x = iX;
            this.y = iY;
        }

		public MapPoint(float iX, float iY)
		{
			this.x = Mathf.RoundToInt(iX);
			this.y = Mathf.RoundToInt(iY);
		}

		public MapPoint(Vector2 vector)
		{
			this.x = Mathf.RoundToInt(vector.x);
			this.y = Mathf.RoundToInt(vector.y);
		}
		
		public override int GetHashCode ()
		{
			unchecked
			{
				return 31 * x.GetHashCode()  + y.GetHashCode();
			}
		}
		
		public override bool Equals (object obj)
		{
			MapPoint point = (MapPoint)obj;
			
			return point.x == x && point.y == y;
		}
		
		public bool EqualsTo (MapPoint point)
		{
			if(point == null)
				return false;
						
			return this.x == point.x && this.y == point.y;			
		} 
		
		public override string ToString ()
		{
			return string.Format ("[Point: x={0}, y={1}]", x, y);
		}

		public Vector2 ToVector2()
		{
			return new Vector2(x,y);
		}

        public static MapPoint operator +(MapPoint c1, MapPoint c2)
        {
            return new MapPoint(c1.x + c2.x, c1.y + c2.y);
        }

        public static MapPoint up
        {
            get
            {
                return new MapPoint(0, 1);
            }
        }

        public static MapPoint down
        {
            get
            {
                return new MapPoint(0, -1);
            }
        }

        public static MapPoint left
        {
            get
            {
                return new MapPoint(-1, 0);
            }
        }

        public static MapPoint right
        {
            get
            {
                return new MapPoint(1, 0);
            }
        }
    }

}
