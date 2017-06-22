using System.Collections.Generic;
using Core;

namespace Mechanics.Defense
{
	public sealed class MGPath:Observable
	{
		public readonly List<MapPoint> Points;
		public readonly MapPoint From;
		public readonly MapPoint To;
		
		public MGPath (MapPoint from, MapPoint to)
		{
			Points = new List<MapPoint>();
			From = from;
			To = to;
		}
		
		public bool IsContainsPoint(MapPoint point)
		{
			return Points.Contains(point);
		}
		
		public static string MakeKey(MapPoint from, MapPoint to)
		{
			return string.Format ("{0}.{1}-{2}.{3}",from.x,from.y,to.x,to.y);			
		}
		
		public bool IsEmpty
		{
			get
			{
				return Points.Count == 0;
			}
		}
		
		public override string ToString ()
		{
			return MakeKey(From,To);
		}
	}
}

