using System;
using System.Collections.Generic;

namespace Mechanics.Defense
{
    public sealed class MGNode : IComparable
    {
        public readonly int x;
        public readonly int y;
		
        public readonly bool walkable;
		
        public float heuristicStartToEndDist;
        public float startToCurNodeDist;
        public float heuristicCurNodeToEndLen;
				
        public MGNode parent;
		
        public bool isOpened;
        public bool isClosed;

		
        public MGNode(int iX, int iY, bool isWalkable)
        {
            this.x = iX;
            this.y = iY;
            this.walkable = isWalkable;
            this.isOpened = false;
            this.isClosed = false;
            this.parent = null;
        }

       /* public void Reset(bool iWalkable)
        {
            this.walkable = iWalkable;
			
            this.heuristicStartToEndDist = 0;
            this.startToCurNodeDist = 0;
            this.heuristicCurNodeToEndLen = 0;
            this.isOpened = false;
            this.isClosed = false;
            this.parent = null;
        }*/
		
		public MGNode Clone()
		{
			MGNode node = new MGNode(this.x,this.x,this.walkable);
			
			node.heuristicStartToEndDist = this.heuristicStartToEndDist;
			node.startToCurNodeDist = this.heuristicStartToEndDist;
			node.heuristicCurNodeToEndLen = this.heuristicStartToEndDist;
			node.isOpened = this.isOpened;
			node.isClosed = this.isClosed;
			
			if(this.parent != null)
			{
				node.parent = this.parent.Clone();
			}
			
			return node;
		}

        public int CompareTo(object iObj)
        {
            MGNode tOtherNode = (MGNode)iObj;
            float result = this.heuristicStartToEndDist - tOtherNode.heuristicStartToEndDist;
            if (result > 0.0f)
                return 1;
            else if (result == 0.0f)
                return 0;
            return -1;
        }

        internal static List<MapPoint> Backtrace(MGNode node)
        {
            List<MapPoint> path = new List<MapPoint>();
            path.Add(new MapPoint(node.x, node.y));
            while (node.parent != null)
            {
                node = node.parent;
                path.Add(new MapPoint(node.x, node.y));
            }
            path.Reverse();
            return path;
        }
    }
}
