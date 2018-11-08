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

    List<Vector2Int> getPath(
    Vector2Int origin, 
    Vector2Int target)
    {
        tileGrid.ForEach(v => v.ForEach(t => t.costSoFar = Mathf.Infinity));
        tileGrid.ForEach(v => v.ForEach(t => t.totalEstCost = Mathf.Infinity));
        tileGrid.ForEach(v => v.ForEach(t => t.estDisToB = Vector2.Distance(t.position, target)));
        
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

        List<List<TileClass>> adjacentSquares = new List<List<TileClass>>();
        adjacentSquares.ForEach(v => v = new List<TileClass>());

        adjacentSquares.ForEach(v => v.ForEach(t => t = null));
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
                    adjacentSquares.ForEach(v => v.ForEach(t => t = null));
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

                adjacentSquares.ForEach(v => v.ForEach(t => t = null));
                getNewAdjacentListSquare(adjacentSquares, aStarData.currentTile.position);
                aStarData.currentTile = getLowestAdjSquare(adjacentSquares);

            }
        }
        return path;
    }

    private TileClass getLowestAdjSquare(List<List<TileClass>> adjacentSquares)
    {
        TileClass lowest = null;
        foreach (List<TileClass> tileClasses in tileGrid)
        {
            foreach (TileClass tileClass in tileClasses.Where(u => isValidTile(u)))
            {
                if(lowest==null || tileClass.totalEstCost < lowest.totalEstCost)
                {
                    lowest = tileClass;
                }
            }
        }
        return lowest;
    }



    //BEGIN GENZO CODE

    class AStarData
    {
        public HashSet<TileClass> openList;
        public HashSet<TileClass> closedList;
        public TileClass currentTile;
    }


    private bool InclusiveBounds(
    int min, 
    int max, 
    int value)
    {
        return (value >= min) && (value <= max);
    }


    private bool isValidTile(TileClass neighborTile)
    {
        return neighborTile != null;
    }

    //END GENZO CODE

        

    private void adjacentSquaresToOpenList(
    List<List<TileClass>> adjSquares,
    AStarData currentData)
    {
        foreach(List<TileClass> tileClasses in tileGrid)
        {
            foreach(TileClass tileClass in tileClasses.Where(u=> isValidTile(u)))
            {
                updateAdjTiles(tileClass, currentData);
            }
        }
    }

    void updateAdjTiles(
    TileClass curNeighbor,
    AStarData curData)
    {
        if (curData.closedList.Contains(curNeighbor) == false)
        {
            if (curData.currentTile.costSoFar + curNeighbor.cost < curNeighbor.costSoFar)
            {
                curNeighbor.costSoFar = curData.currentTile.costSoFar + curNeighbor.cost;
            }
            if (curNeighbor.totalEstCost > curNeighbor.estDisToB + curNeighbor.costSoFar)
            {
                curNeighbor.totalEstCost = curNeighbor.estDisToB + curNeighbor.costSoFar;

            }

            if(curData.openList.Contains(curNeighbor) == false)
            {
                curData.openList.Add(curNeighbor);
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

    void getNewAdjacentListSquare(List<List<TileClass>> adjacentSquares, Vector2Int position)
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
                            adjacentSquares[row+1][col+1] = tileGrid[row + position.y][col + position.x];
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
