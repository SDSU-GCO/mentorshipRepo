using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;
using System.Linq;

public class TileMapController : MonoBehaviour {

    public TileMapSceneSupervisor tileMapSupervisor = null;
    Sprite sprite;

    class TileClass
    {
        public bool visted;
        public bool traverseable;
        public float cost;
        public float costSoFar;
        public float estDisToB;
        public float totalEstCost;
        public Vector2Int position;
    }

    class AdjTileClassContainer
    {
        public TileClass tile;
        public bool isCorner;
    }

    List<List<TileClass>> tileGrid = new List<List<TileClass>>();


    private void Reset()
    {
        tileMapSupervisor = FindObjectOfType<TileMapSceneSupervisor>();
    }
    
    private void OnEnable()
    {
        Debug.Assert(tileMapSupervisor != null, "Error: " + this + " has no reference to a tileMapSupervisor!");
        tileMapSupervisor.tileMapControllers.Add(this);
        InitializeGrid();
    }

    private void OnDisable()
    {
        if(tileMapSupervisor!=null)
        {
            tileMapSupervisor.tileMapControllers.Remove(this);
        }
    }
    
    void InitializeGrid()
    {
        TileClass tempTile;
        Vector2Int boundry = getMaxSizes();
        tileGrid.Capacity = boundry.x;
        for (int r = 0; r < boundry.x; r++)
        {
            tileGrid[r].Capacity = boundry.y;
            for (int c = 0; c < boundry.y; c++)
            {
                getTile(out tempTile, c, r);
                tileGrid[r][c] = tempTile;
            }
        }
    }

    float getOctileDistacne(Vector2Int origin, Vector2Int target)
    {
        float axisLockedPart = 0;
        float diagonalLockedPart = 0;
        Vector2Int resultantVector = origin - target;
        if(Mathf.Abs( resultantVector.x) > Mathf.Abs(resultantVector.y))
        {
            axisLockedPart = Mathf.Abs(resultantVector.x) - Mathf.Abs(resultantVector.y);
            diagonalLockedPart = Mathf.Sqrt(Mathf.Abs(resultantVector.y) + Mathf.Abs(resultantVector.y));
        }
        else
        {
            axisLockedPart = Mathf.Abs(resultantVector.y) - Mathf.Abs(resultantVector.x);
            diagonalLockedPart = Mathf.Sqrt(Mathf.Abs(resultantVector.x) + Mathf.Abs(resultantVector.x));
        }
        return axisLockedPart + diagonalLockedPart;
    }

    List<Vector2Int> getPath(
    Vector2Int origin, 
    Vector2Int target)
    {
        tileGrid.ForEach(v => v.ForEach(t => t.costSoFar = Mathf.Infinity));
        tileGrid.ForEach(v => v.ForEach(t => t.totalEstCost = Mathf.Infinity));
        tileGrid.ForEach(v => v.ForEach(t => t.estDisToB = getOctileDistacne(t.position, target)));
        
        AStarData aStarData = new AStarData();
        aStarData.openList = new HashSet<TileClass>();
        aStarData.closedList = new HashSet<TileClass>();

        aStarData.currentTile = tileGrid[origin.y][origin.x];
        aStarData.currentTile.visted = true;
        aStarData.currentTile.costSoFar = 0;
        aStarData.currentTile.totalEstCost = aStarData.currentTile.estDisToB;
        aStarData.closedList.Add(aStarData.currentTile);

        TileClass targetTile = tileGrid[target.y][target.x];
        TileClass originTile = tileGrid[origin.y][origin.x];

        List<List<AdjTileClassContainer>> adjacentSquares = new List<List<AdjTileClassContainer>>();
        adjacentSquares.ForEach(v => v = new List<AdjTileClassContainer>());

        adjacentSquares.ForEach(v => v.ForEach(a => a = new AdjTileClassContainer()));
        adjacentSquares.ForEach(v => v.ForEach(a => a.tile = null));
        adjacentSquares.ForEach(v => v.ForEach(a => a.isCorner = false));
        getNewAdjacentListSquare(adjacentSquares, origin);

        bool pathNotFound = true;

        adjacentSquaresToOpenList(adjacentSquares, aStarData);

        while (pathNotFound && aStarData.currentTile!=null)
        {
            TileClass minTile = null;
            foreach(TileClass tile in aStarData.openList)
            {
                if(minTile == null || minTile.totalEstCost < tile.totalEstCost)
                {
                    minTile = tile;
                }
            }
            aStarData.currentTile = minTile;
            if(aStarData.currentTile != null)
            {
                aStarData.currentTile.visted = true;
                aStarData.currentTile.totalEstCost = aStarData.currentTile.estDisToB + aStarData.currentTile.costSoFar;
                aStarData.closedList.Add(aStarData.currentTile);


                if(aStarData.currentTile == targetTile)
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
                        if (nextNode == null || tileClass.totalEstCost < nextNode.totalEstCost)
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

        List<Vector2Int> path = new List<Vector2Int>();
        if (pathNotFound==false)
        {
            while(aStarData.currentTile!= originTile)
            {
                path.Add(aStarData.currentTile.position);

                adjacentSquares.ForEach(v => v.ForEach(a => a.tile = null));
                getNewAdjacentListSquare(adjacentSquares, aStarData.currentTile.position);
                aStarData.currentTile = getLowestAdjSquare(adjacentSquares);
            }
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
                if(lowest==null || adjTileClassContainer.tile.totalEstCost < lowest.totalEstCost)
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
        foreach(List<AdjTileClassContainer> adjTileClassContainers in adjSquares)
        {
            foreach(AdjTileClassContainer adjTileClassContainer in adjTileClassContainers.Where(u=> isValidTile(u.tile)))
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
    AdjTileClassContainer curNeighbor,
    AStarData curData)
    {
        if (curData.closedList.Contains(curNeighbor.tile) == false)
        {
            if(curNeighbor.isCorner==false)
            {
                if (curData.currentTile.costSoFar + curNeighbor.tile.cost < curNeighbor.tile.costSoFar)
                {
                    curNeighbor.tile.costSoFar = curData.currentTile.costSoFar + curNeighbor.tile.cost;
                }
                if (curNeighbor.tile.totalEstCost > curNeighbor.tile.estDisToB + curNeighbor.tile.costSoFar)
                {
                    curNeighbor.tile.totalEstCost = curNeighbor.tile.estDisToB + curNeighbor.tile.costSoFar;
                }
            }
            else
            {
                //use octile distance for diagonals
                float squareRootOfTwo = Mathf.Sqrt(2);
                if ((curData.currentTile.costSoFar + curNeighbor.tile.cost)* squareRootOfTwo < curNeighbor.tile.costSoFar)
                {
                    curNeighbor.tile.costSoFar = (curData.currentTile.costSoFar + curNeighbor.tile.cost) * squareRootOfTwo;
                }
                if (curNeighbor.tile.totalEstCost > curNeighbor.tile.estDisToB + curNeighbor.tile.costSoFar)
                {
                    curNeighbor.tile.totalEstCost = curNeighbor.tile.estDisToB + curNeighbor.tile.costSoFar;
                }
            }

            if(curData.openList.Contains(curNeighbor.tile) == false)
            {
                curData.openList.Add(curNeighbor.tile);
            }
        }
    }

    public Vector2Int? getNextTile(Vector2Int origin, Vector2Int target)
    {
        Vector2Int? nextTile;
        List<Vector2Int> path = getPath(origin, target);
        if(path.Count>0)
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
    Vector2Int position)
    {
        for(int row = -1; row <= 1; row++)
        {
            if (0 <= row + position.y && row + position.y < tileGrid.Count)
            {
                for (int col = -1; col <= 1; col++)
                {
                    if (0 <= col + position.x && col + position.x < tileGrid[0].Count)
                    {
                        if(!(row==0 && col ==0))
                        {
                            adjSquares[row+1][col+1].tile = tileGrid[row + position.y][col + position.x];
                            if(row!=0 && col !=0)
                            {
                                adjSquares[row + 1][col + 1].isCorner = true;
                            }
                            else
                            {
                                adjSquares[row + 1][col + 1].isCorner = false;
                            }
                        }
                    }
                }
            }
        }
    }

    void getTile<T>(out TileClass tile, int x, int y, T sourceMap) where T : IColorEncodedTileMap
    {
        tile = new TileClass();
        Color tileData = sourceMap.getPixel(x, y);
        if (tileData.r == 1)
        {
            tile.traverseable = false;
        }
        else
        {
            tile.traverseable = true;
        }
        tile.visted = false;
        tile.cost = tileData.r;
        tile.position.x = x;
        tile.position.y = y;
    }

    Vector2Int getMaxSizes<T>(T sourceMap) where T : IColorEncodedTileMap
    {
        return sourceMap.getMaxSizes();
    }

    interface IColorEncodedTileMap
    {
        Vector2Int getMaxSizes();
        Color getPixel(int x, int y);
    }
}
