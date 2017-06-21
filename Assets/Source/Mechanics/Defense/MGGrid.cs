using System.Collections.Generic;
using Core;

namespace Mechanics.Defense
{
    public sealed class MGGrid:Observable
    {
        private int _width;
        private int _height;
        private MGNode[,] _nodes;

		public int Width
		{
			get
			{
				return _width;
			}
		}


		public int Height
		{
			get
			{
				return _height;
			}
		}
        public MGGrid(int[,] matrix, List<int> free)
        {
            _width = matrix.GetUpperBound(0);
            _height = matrix.GetUpperBound(1);
			
            _nodes = BuildNodes(_width, _height, free, matrix);
        }

        public bool IsWalkableAt(int x, int y)
        {
			if(x >= _width || y >= _height)
				return false;

			return IsInside(x, y) && _nodes[x,y].walkable;
        }

        public MGNode GetNodeAt(int x, int y)
        {
            return _nodes[x,y];
        }

       /* public void SetWalkableAt(int x, int y, bool walkable)
        {
            _nodes[x,y].walkable = walkable;
        }*/

        public List<MGNode> GetNeighbors(MGNode iNode)
        {
            int tX = iNode.x;
            int tY = iNode.y;
            List<MGNode> neighbors = new List<MGNode>();
            bool tS0 = false, tD0 = false,
                tS1 = false, tD1 = false,
                tS2 = false, tD2 = false,
                tS3 = false, tD3 = false;

            MapPoint pos = new MapPoint(tX, tY - 1);

            if (this.IsWalkableAt(pos.x, pos.y))
            {
                neighbors.Add(GetNodeAt(pos.x, pos.y));
                tS0 = true;
            }
            pos = new MapPoint(tX + 1, tY);
            if (this.IsWalkableAt(pos.x, pos.y))
            {
                neighbors.Add(GetNodeAt(pos.x, pos.y));
                tS1 = true;
            }
            pos = new MapPoint(tX, tY + 1);
            if (this.IsWalkableAt(pos.x, pos.y))
            {
                neighbors.Add(GetNodeAt(pos.x, pos.y));
                tS2 = true;
            }
            pos = new MapPoint(tX - 1, tY);
            if (this.IsWalkableAt(pos.x, pos.y))
            {
                neighbors.Add(GetNodeAt(pos.x, pos.y));
                tS3 = true;
            }

            tD0 = tS3 || tS0;
            tD1 = tS0 || tS1;
            tD2 = tS1 || tS2;
            tD3 = tS2 || tS3;

            pos = new MapPoint(tX - 1, tY - 1);
            if (tD0 && this.IsWalkableAt(pos.x, pos.y))
            {
                neighbors.Add(GetNodeAt(pos.x, pos.y));
            }
            pos = new MapPoint(tX + 1, tY - 1);
            if (tD1 && this.IsWalkableAt(pos.x, pos.y))
            {
                neighbors.Add(GetNodeAt(pos.x, pos.y));
            }
            pos = new MapPoint(tX + 1, tY + 1);
            if (tD2 && this.IsWalkableAt(pos.x, pos.y))
            {
                neighbors.Add(GetNodeAt(pos.x, pos.y));
            }
            pos = new MapPoint(tX - 1, tY + 1);
            if (tD3 && this.IsWalkableAt(pos.x, pos.y))
            {
                neighbors.Add(GetNodeAt(pos.x, pos.y));
            }
            return neighbors;
        }

        private MGNode[,] BuildNodes(int width, int height, List<int> free, int[,] matrix )
        {
            MGNode[,] nodes = new MGNode[width,height];

            for (int x = 0; x < width; x++)
            {           
                for (int y = 0; y < height; y++)
                {
                    nodes[x,y] = new MGNode(x, y, free.Contains( matrix[x,y] ));
                }
            }

            if (matrix == null)
            {
                return nodes;
            }
			
            return nodes;
        }

        private bool IsInside(int x, int y)
        {
            return (x >= 0 && x < _width) && (y >= 0 && y < _height);
        }

    }
}
