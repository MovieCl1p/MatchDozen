using System;
using System.Collections.Generic;
using Core;

namespace Mechanics.Defense
{
    public sealed class MGPathFinder:MonoScheduledBehaviour
    {
#if PATH_FIND_CACHE
		private readonly Dictionary<string,MGPath> _paths;
		
		private readonly Queue<MGPath> _recalculationQueue;
#endif
		private int[,] _matrix;
		
		private List<int> _free;
		
		public MGPathFinder()
		{
#if PATH_FIND_CACHE
			_paths = new Dictionary<string, MGPath>();
			_recalculationQueue = new Queue<MGPath>();
#endif
		}		
		
		public void Initialize(int[,] matrix, List<int> free)
		{
			_matrix = matrix;
			_free = free;
#if PATH_FIND_CACHE

			_paths.Clear();
			_recalculationQueue.Clear();
#endif
		}		
		
		public void Invalidate(MapPoint point)
		{
#if PATH_FIND_CACHE
			MGPath[] items	= _paths.Values.ToArray();
			
			for(int i = 0; i < items.Count(); i++)
			{
				if(items[i].IsContainsPoint(point) && !_recalculationQueue.Contains(items[i]))
				{
					_recalculationQueue.Enqueue(items[i]);
				}
			}
			
			if(_recalculationQueue.Count > 0)
				ScheduleUpdate(0.1f,false);
#endif
		}

#if PATH_FIND_CACHE		
		protected override void OnScheduledUpdate ()
		{
			MGPath path = _recalculationQueue.Dequeue();
			
			path.Points.Clear();
			path.Points.AddRange( FindPoints( path.From, path.To ) );
			
			if(path.IsEmpty)
			{
				_paths.Remove(MGPath.MakeKey(path.From,path.To));				
			}
			
			path.SetChanged();
		}
#endif		
		
        public static float Manhattan(int iDx, int iDy)
        {
            return (float)iDx + iDy;
        }
		
		private List<MapPoint> FindPoints(MapPoint startPos, MapPoint endPos)
		{
			List<MGNode> openList = new List<MGNode>();
			MGGrid searchGrid = new MGGrid(_matrix,_free);


#if UNITY_EDITOR
			if(startPos.x >= searchGrid.Width || startPos.y >= searchGrid.Height)
			{
				throw new Exception("Invalid start position " + startPos + " out bounds is " + searchGrid.Width + "x" + searchGrid.Height);
			}

			if(endPos.x >= searchGrid.Width  || endPos.y >= searchGrid.Height)
			{
				throw new Exception("Invalid end position " + startPos);
			}

#endif
            MGNode startNode = searchGrid.GetNodeAt(startPos.x, startPos.y);
            MGNode endNode = searchGrid.GetNodeAt(endPos.x, endPos.y);
            MGNode node;

            startNode.startToCurNodeDist = 0;
            startNode.heuristicStartToEndDist = 0;
            startNode.startToCurNodeDist = 0;

            openList.Add(startNode);
            startNode.isOpened = true;

            while (openList.Count > 0)
            {
                
                openList.Sort();
                node = openList[0];
                openList.RemoveAt(0);
                node.isClosed = true;
                
                if (node.x == endNode.x && node.y == endNode.y)
                {
                    return MGNode.Backtrace(endNode);
                }

                IdentifySuccessors(openList, searchGrid, endNode, node);
            }		

			return new List<MapPoint>();
		}
		
        public MGPath FindPath(MapPoint startPos, MapPoint endPos)
        {		
#if PATH_FIND_CACHE
			string pathKey = MGPath.MakeKey(startPos,endPos);
			
			if(_paths.ContainsKey(pathKey))
			{
				return _paths[pathKey];
			}
#endif
			
			MGPath path = new MGPath(startPos,endPos);
			
            path.Points.AddRange( FindPoints( startPos, endPos ) );

#if PATH_FIND_CACHE
			if(!path.IsEmpty)				
				_paths.Add(pathKey,path);			
#endif
			
            return path;
        }

        private void IdentifySuccessors(List<MGNode> nodeList, MGGrid searchGrid, MGNode endNode, MGNode iNode)
        {
            List<MGNode> openList = nodeList;
            int endX = endNode.x;
            int endY = endNode.y;
            MapPoint neighbor;
            MapPoint jumpPoint;
            MGNode jumpNode;

            List<MapPoint> neighbors = FindNeighbors(iNode, searchGrid);
            
            for (int i = 0; i < neighbors.Count; i++)
            {
                neighbor = neighbors[i];
                jumpPoint = Jump(endNode, neighbor.x, neighbor.y, iNode.x, iNode.y, searchGrid);
                if (jumpPoint != null)
                {
                    jumpNode = searchGrid.GetNodeAt(jumpPoint.x, jumpPoint.y);

                    if (jumpNode.isClosed)
                    {
                        continue;
                    }
                    // include distance, as parent may not be immediately adjacent:
                    float tCurNodeToJumpNodeLen = Manhattan(Math.Abs(jumpPoint.x - iNode.x), Math.Abs(jumpPoint.y - iNode.y));
                    float tStartToJumpNodeLen = iNode.startToCurNodeDist + tCurNodeToJumpNodeLen; // next `startToCurNodeLen` value

                    if (!jumpNode.isOpened || tStartToJumpNodeLen < jumpNode.startToCurNodeDist)
                    {						
                        jumpNode.startToCurNodeDist = tStartToJumpNodeLen;
                        jumpNode.heuristicCurNodeToEndLen = (jumpNode.heuristicCurNodeToEndLen == 0 ? Manhattan(Math.Abs(jumpPoint.x - endX), Math.Abs(jumpPoint.y - endY)) : jumpNode.heuristicCurNodeToEndLen);
                        jumpNode.heuristicStartToEndDist = jumpNode.startToCurNodeDist + jumpNode.heuristicCurNodeToEndLen;
                        jumpNode.parent = iNode;

                        if (!jumpNode.isOpened)
                        {
                            openList.Add(jumpNode);
                            jumpNode.isOpened = true;
                        }
                    }
                }
            }
        }

        private static List<MapPoint> FindNeighbors(MGNode node, MGGrid searchGrid)
        {
            MGNode tParent = node.parent;
            int tX = node.x;
            int tY = node.y;
            int tPx, tPy, tDx, tDy;
            List<MapPoint> neighbors = new List<MapPoint>();
            List<MGNode> tNeighborNodes;
            MGNode tNeighborNode;

            if (tParent != null)
            {
                tPx = tParent.x;
                tPy = tParent.y;
                
                tDx = (tX - tPx) / Math.Max(Math.Abs(tX - tPx), 1);
                tDy = (tY - tPy) / Math.Max(Math.Abs(tY - tPy), 1);

                // search diagonally
                if (tDx != 0 && tDy != 0)
                {
                    if (searchGrid.IsWalkableAt(tX, tY + tDy))
                    {
                        neighbors.Add(new MapPoint(tX, tY + tDy));
                    }
                    if (searchGrid.IsWalkableAt(tX + tDx, tY))
                    {
                        neighbors.Add(new MapPoint(tX + tDx, tY));
                    }

                    if (searchGrid.IsWalkableAt(tX + tDx, tY + tDy))
                    {
                        if (searchGrid.IsWalkableAt(tX, tY + tDy) || searchGrid.IsWalkableAt(tX + tDx, tY))
                        {
                            neighbors.Add(new MapPoint(tX + tDx, tY + tDy));
                        }
                    }

                    if (!searchGrid.IsWalkableAt(tX - tDx, tY) && searchGrid.IsWalkableAt(tX - tDx, tY + tDy))
                    {
                        if (searchGrid.IsWalkableAt(tX, tY + tDy))
                        {
                            neighbors.Add(new MapPoint(tX - tDx, tY + tDy));
                        }
                    }

                    if (!searchGrid.IsWalkableAt(tX, tY - tDy) && searchGrid.IsWalkableAt(tX + tDx, tY - tDy))
                    {
                        if (searchGrid.IsWalkableAt(tX + tDx, tY))
                        {
                            neighbors.Add(new MapPoint(tX + tDx, tY - tDy));
                        }
                    }


                }
                // search horizontally/vertically
                else
                {
                    if (tDx == 0)
                    {
                        if (searchGrid.IsWalkableAt(tX, tY + tDy))
                        {
                            neighbors.Add(new MapPoint(tX, tY + tDy));

                            if (!searchGrid.IsWalkableAt(tX + 1, tY) && searchGrid.IsWalkableAt(tX + 1, tY + tDy))
                            {
                                neighbors.Add(new MapPoint(tX + 1, tY + tDy));
                            }
                            if (!searchGrid.IsWalkableAt(tX - 1, tY) && searchGrid.IsWalkableAt(tX - 1, tY + tDy))
                            {
                                neighbors.Add(new MapPoint(tX - 1, tY + tDy));
                            }
                        }
                    }
                    else
                    {
                        if (searchGrid.IsWalkableAt(tX + tDx, tY))
                        {

                            neighbors.Add(new MapPoint(tX + tDx, tY));

                            if (!searchGrid.IsWalkableAt(tX, tY + 1) && searchGrid.IsWalkableAt(tX + tDx, tY + 1))
                            {
                                neighbors.Add(new MapPoint(tX + tDx, tY + 1));
                            }
                            if (!searchGrid.IsWalkableAt(tX, tY - 1) && searchGrid.IsWalkableAt(tX + tDx, tY - 1))
                            {
                                neighbors.Add(new MapPoint(tX + tDx, tY - 1));
                            }
                        }
                    }
                }
            }
            // return all neighbors
            else
            {
                tNeighborNodes = searchGrid.GetNeighbors(node);
                for (int i = 0; i < tNeighborNodes.Count; i++)
                {
                    tNeighborNode = tNeighborNodes[i];
                    neighbors.Add(new MapPoint(tNeighborNode.x, tNeighborNode.y));
                }
            }

            return neighbors;
        }

        private static MapPoint Jump(MGNode endNode, int x, int y, int Px, int Py, MGGrid searchGrid)
        {
            if (!searchGrid.IsWalkableAt(x, y))
            {
                return null;
            }
            else if (searchGrid.GetNodeAt(x, y).Equals(endNode))
            {
                return new MapPoint(x, y);
            }
            int tDx = x - Px;
            int tDy = y - Py;

            // check for forced neighbors
            // along the diagonal
            if (tDx != 0 && tDy != 0)
            {
                if ((searchGrid.IsWalkableAt(x - tDx, y + tDy) && !searchGrid.IsWalkableAt(x - tDx, y)) ||
                    (searchGrid.IsWalkableAt(x + tDx, y - tDy) && !searchGrid.IsWalkableAt(x, y - tDy)))
                {
                    return new MapPoint(x, y);
                }
            }
            // horizontally/vertically
            else
            {
                if (tDx != 0)
                { // moving along x
                    if ((searchGrid.IsWalkableAt(x + tDx, y + 1) && !searchGrid.IsWalkableAt(x, y + 1)) ||
                        (searchGrid.IsWalkableAt(x + tDx, y - 1) && !searchGrid.IsWalkableAt(x, y - 1)))
                    {
                        return new MapPoint(x, y);
                    }
                }
                else
                {
                    if ((searchGrid.IsWalkableAt(x + 1, y + tDy) && !searchGrid.IsWalkableAt(x + 1, y)) ||
                        (searchGrid.IsWalkableAt(x - 1, y + tDy) && !searchGrid.IsWalkableAt(x - 1, y)))
                    {
                        return new MapPoint(x, y);
                    }
                }
            }

            MapPoint jx;
            MapPoint jy;
            // when moving diagonally, must check for vertical/horizontal jump points
            if (tDx != 0 && tDy != 0)
            {
                jx = Jump(endNode, x + tDx, y, x, y, searchGrid);
                jy = Jump(endNode, x, y + tDy, x, y, searchGrid);
                if (jx != null || jy != null)
                {
                    return new MapPoint(x, y);
                }
            }

            // moving diagonally, must make sure one of the vertical/horizontal
            // neighbors is open to allow the path
            if (searchGrid.IsWalkableAt(x + tDx, y) || searchGrid.IsWalkableAt(x, y + tDy))
            {
                return Jump(endNode, x + tDx, y + tDy, x, y, searchGrid);
            }
            else
            {
                return null;
            }
        }
    
    }
}
