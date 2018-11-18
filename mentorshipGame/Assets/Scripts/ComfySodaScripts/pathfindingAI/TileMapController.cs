using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;
using System.Linq;
using UnityEngine.Tilemaps;
using UnityEditor;

namespace cs
{
    [RequireComponent(typeof(Tilemap))]
    public class TileMapController : MonoBehaviour
    {
        public TileMapSceneSupervisor tileMapSupervisor = null;
        Sprite sprite;
        Tilemap tilemap=null;
        

        private void Awake()
        {
            if(tilemap==null)
                tilemap = GetComponent<Tilemap>();
        }

        class TileClass
        {
            public bool visited;
            public bool traverseable;
            public float traverseCost;
            public float cumulativeCost;
            public float estDistanceToEnd;
            public float totalEstCostToEnd;
            public Vector3Int position;
        }

        class AdjTileClassContainer
        {
            public TileClass tile;
            public bool isDiagonalToCurrentTile;
        }

        List<List<TileClass>> tileGrid = new List<List<TileClass>>();


        private void Reset()
        {
            tileMapSupervisor = FindObjectOfType<TileMapSceneSupervisor>();
            tilemap = GetComponent<Tilemap>();
        }

        private void OnEnable()
        {
            Debug.Assert(tileMapSupervisor != null, "Error: " + this + " has no reference to a tileMapSupervisor!");
            tileMapSupervisor.tileMapControllers.Add(this);
            InitializeGrid();
        }

        private void OnDisable()
        {
            if (tileMapSupervisor != null)
            {
                tileMapSupervisor.tileMapControllers.Remove(this);
            }
        }

        void InitializeGrid()
        {
            TileClass tempTile;
            BoundsInt boundry = getMaxSizes();
            tileGrid.Capacity = boundry.x;
            int offsetFromX = -boundry.yMin;
            int offsetFromY = -boundry.xMin;
            for (int tilePosY = boundry.xMin; tilePosY < boundry.xMax; tilePosY++)
            {
                tileGrid[tilePosY].Capacity = boundry.y;
                for (int tilePosX = boundry.yMin; tilePosX < boundry.yMax; tilePosX++)
                {
                    getTile(out tempTile, tilePosX, tilePosY);
                    tileGrid[tilePosY + offsetFromY][tilePosX + offsetFromX] = tempTile;
                }
            }
        }

        float getOctileDistance(Vector3Int origin, Vector3Int target)
        {
            Vector3Int trajectory = (target - origin);
            trajectory.y = Mathf.Abs(trajectory.y);
            trajectory.x = Mathf.Abs(trajectory.x);
            int minAxis = Mathf.Min(trajectory.x, trajectory.y);
            int cardinal = Mathf.Max(trajectory.x, trajectory.y) - minAxis;
            float diagonal = minAxis * Mathf.Sqrt(2.0f);
            return diagonal + cardinal;
        }

        public List<Vector3Int> getPath(
        Vector3Int origin,
        Vector3Int target)
        {
            //initiializes every tile grid's cumulative cost and total estimated cost to end to infinity.
            //these fields will be updated once the traversing requires access to these grids.
            //estimated distance to end is pre-calculated through octile distance function, to check later if we are going the right way.
            tileGrid.ForEach(v => v.ForEach(t => t.cumulativeCost = Mathf.Infinity));
            tileGrid.ForEach(v => v.ForEach(t => t.totalEstCostToEnd = Mathf.Infinity));
            tileGrid.ForEach(v => v.ForEach(t => t.estDistanceToEnd = getOctileDistance(t.position, target)));

            AStarData aStarData = new AStarData();
            aStarData.openList = new HashSet<TileClass>();
            aStarData.closedList = new HashSet<TileClass>();

            aStarData.currentTile = tileGrid[origin.y][origin.x];
            aStarData.currentTile.visited = true;
            aStarData.currentTile.cumulativeCost = 0;
            aStarData.currentTile.totalEstCostToEnd = aStarData.currentTile.estDistanceToEnd;
            aStarData.closedList.Add(aStarData.currentTile);

            TileClass targetTile = tileGrid[target.y][target.x];
            TileClass originTile = tileGrid[origin.y][origin.x];

            List<List<AdjTileClassContainer>> adjacentSquares = new List<List<AdjTileClassContainer>>();
            adjacentSquares.ForEach(v => v = new List<AdjTileClassContainer>());

            adjacentSquares.ForEach(v => v.ForEach(a => a = new AdjTileClassContainer()));
            adjacentSquares.ForEach(v => v.ForEach(a => a.tile = null));
            adjacentSquares.ForEach(v => v.ForEach(a => a.isDiagonalToCurrentTile = false));
            getNewAdjacentListSquare(adjacentSquares, origin);

            bool pathNotFound = true;

            adjacentSquaresToOpenList(adjacentSquares, aStarData);

            while (pathNotFound && aStarData.currentTile != null)
            {
                TileClass minTile = null;
                foreach (TileClass tile in aStarData.openList)
                {
                    if (minTile == null || minTile.totalEstCostToEnd < tile.totalEstCostToEnd)
                    {
                        minTile = tile;
                    }
                }
                aStarData.currentTile = minTile;
                if (aStarData.currentTile != null)
                {
                    aStarData.currentTile.visited = true;
                    aStarData.currentTile.totalEstCostToEnd = aStarData.currentTile.estDistanceToEnd + aStarData.currentTile.cumulativeCost;
                    aStarData.closedList.Add(aStarData.currentTile);


                    if (aStarData.currentTile == targetTile)
                    {
                        pathNotFound = false;
                    }
                    else
                    {
                        adjacentSquares.ForEach(v => v.ForEach(a => a.tile = null));
                        getNewAdjacentListSquare(adjacentSquares, aStarData.currentTile.position);
                        adjacentSquaresToOpenList(adjacentSquares, aStarData);

                        TileClass nextNode = null;
                        foreach (TileClass tileClass in aStarData.openList)
                        {
                            if (nextNode == null || tileClass.totalEstCostToEnd < nextNode.totalEstCostToEnd)
                            {
                                nextNode = tileClass;
                            }
                        }
                        if (nextNode != null)
                        {
                            aStarData.openList.Remove(nextNode);
                            aStarData.closedList.Add(nextNode);
                        }
                        aStarData.currentTile = nextNode;
                    }
                }

            }

            List<Vector3Int> path = new List<Vector3Int>();
            if (pathNotFound == false)
            {
                while (aStarData.currentTile != originTile)
                {
                    path.Add(aStarData.currentTile.position);

                    adjacentSquares.ForEach(v => v.ForEach(a => a.tile = null));
                    getNewAdjacentListSquare(adjacentSquares, aStarData.currentTile.position);
                    aStarData.currentTile = getLowestAdjSquare(adjacentSquares);
                }
            }
            else
            {
                path = null;
            }
            return path;
        }

        private TileClass getLowestAdjSquare(List<List<AdjTileClassContainer>> adjacentSquares)
        {
            TileClass lowest = null;
            foreach (List<AdjTileClassContainer> adjTileClassContainers in adjacentSquares)
            {
                foreach (AdjTileClassContainer adjTileClassContainer in adjTileClassContainers.Where(u => isValidTile(u.tile)))
                {
                    if (lowest == null || adjTileClassContainer.tile.totalEstCostToEnd < lowest.totalEstCostToEnd)
                    {
                        lowest = adjTileClassContainer.tile;
                    }
                }
            }
            return lowest;
        }



        //BEGIN GENZO CODE
        /// <summary>
        /// Bundle aStar data to reduce argument list madness
        /// </summary>
        class AStarData
        {
            public HashSet<TileClass> openList;
            public HashSet<TileClass> closedList;
            public TileClass currentTile;
        }

        /// <summary>
        /// check if a tile is valid
        /// </summary>
        /// <param name="neighborTile"></param>
        /// <returns></returns>
        private bool isValidTile(TileClass neighborTile)
        {
            return neighborTile != null;
        }

        //END GENZO CODE


        /// <summary>
        /// add valid adjacent squares to the open list
        /// </summary>
        /// <param name="adjSquares"></param>
        /// <param name="currentData"></param>
        private void adjacentSquaresToOpenList(
        List<List<AdjTileClassContainer>> adjSquares,
        AStarData currentData)
        {
            foreach (List<AdjTileClassContainer> adjTileClassContainers in adjSquares)
            {
                foreach (AdjTileClassContainer adjTileClassContainer in adjTileClassContainers.Where(u => isValidTile(u.tile)))
                {
                    updateAdjTiles(adjTileClassContainer, currentData);
                }
            }
        }

        /// <summary>
        /// Calculate the distance & cost to adjacent tiles using octile distance
        /// </summary>
        /// <param name="curNeighbor"></param>
        /// <param name="curData"></param>
        void updateAdjTiles(
        AdjTileClassContainer curryNeighbor,
        AStarData curryData)
        {
            if (curryData.closedList.Contains(curryNeighbor.tile) == false)
            {
                //if the current neighboring tile is not diagonally positioned to the current tile
                if (curryNeighbor.isDiagonalToCurrentTile == false)
                {
                    //if the current cumulative cost to the current tile and the cost to the neighboring tile is less than 
                    //cumulative cost in the neighboring tile (set to infinity initially)
                    if ((curryData.currentTile.cumulativeCost + curryNeighbor.tile.traverseCost) <
                        curryNeighbor.tile.cumulativeCost)
                    {
                        //set the new cumulative cost to traverse
                        curryNeighbor.tile.cumulativeCost = curryData.currentTile.cumulativeCost
                            + curryNeighbor.tile.traverseCost;
                    }
                    //if the total esimated cost 
                    if (curryNeighbor.tile.totalEstCostToEnd >
                        curryNeighbor.tile.estDistanceToEnd + curryNeighbor.tile.cumulativeCost)
                    {
                        curryNeighbor.tile.totalEstCostToEnd = curryNeighbor.tile.estDistanceToEnd
                            + curryNeighbor.tile.cumulativeCost;
                    }
                }
                //if the current tile is diagonally positioned to the current tile
                else
                {
                    //use octile distance for diagonals
                    float sqrtTwo = Mathf.Sqrt(2);
                    //if the current cumulative cost to the current tile and the cost to the neighboring tile, multiplied by 
                    //square root of two is less than the cumulative cost in the neighboring tile (set to infinity initially)
                    if ((curryData.currentTile.cumulativeCost + curryNeighbor.tile.traverseCost) * sqrtTwo <
                        curryNeighbor.tile.cumulativeCost)
                    {
                        curryNeighbor.tile.cumulativeCost = (curryData.currentTile.cumulativeCost
                            + curryNeighbor.tile.traverseCost) * sqrtTwo;
                    }
                    if (curryNeighbor.tile.totalEstCostToEnd > curryNeighbor.tile.estDistanceToEnd
                        + curryNeighbor.tile.cumulativeCost)
                    {
                        curryNeighbor.tile.totalEstCostToEnd = curryNeighbor.tile.estDistanceToEnd
                            + curryNeighbor.tile.cumulativeCost;
                    }
                }
                //if the current data doesn't include the neighboring tile
                if (curryData.openList.Contains(curryNeighbor.tile) == false)
                {
                    //include the neighboring tile
                    curryData.openList.Add(curryNeighbor.tile);
                }
            }
        }

        public Vector3Int? getNextTile(Vector3Int origin, Vector3Int target)
        {
            Vector3Int? nextTile;
            List<Vector3Int> path = getPath(origin, target);
            if (path!=null && path.Count > 0)
            {
                nextTile = path[0];
            }
            else
            {
                nextTile = null;
            }
            return nextTile;
        }

        void getNewAdjacentListSquare(
        List<List<AdjTileClassContainer>> adjSquares,
        Vector3Int position)
        {
            for (int row = -1; row <= 1; row++)
            {
                if ((row + position.y) >= 0 && (row + position.y) < tileGrid.Count)
                {
                    for (int col = -1; col <= 1; col++)
                    {
                        if (0 <= col + position.x && col + position.x < tileGrid[0].Count)
                        {
                            if (!(row == 0 && col == 0))
                            {
                                adjSquares[row + 1][col + 1].tile = tileGrid[row + position.y][col + position.x];
                                if (row != 0 && col != 0)
                                {
                                    adjSquares[row + 1][col + 1].isDiagonalToCurrentTile = true;
                                }
                                else
                                {
                                    adjSquares[row + 1][col + 1].isDiagonalToCurrentTile = false;
                                }
                            }
                        }
                    }
                }
            }
        }

        void getTile(out TileClass tile, int x, int y)
        {
            tile = new TileClass();


            tile.traverseable = tilemap.HasTile(new Vector3Int(x, y, 0));
            tile.visited = false;
            tile.traverseCost = 1;
            tile.position = new Vector3Int
            {
                x = x,
                y = y,
                z = 0,
            };
        }

        BoundsInt getMaxSizes()
        {
            return (tilemap.cellBounds);
        }
    }
}