using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;

public class TileMapController : MonoBehaviour {

    public TileMapSceneSupervisor tileMapSupervisor = null;
    Sprite sprite;

    class TileClass
    {
        public bool visted;
        public bool traverseable;
        public float cost;
        public float costSoFar;
        public float h;
        public float f;
        public Vector2Int position;
        
        public static implicit operator Tile(TileClass obj)
        {
            Tile tile;
            tile.visted = obj.visted;
            tile.traverseable = obj.traverseable;
            tile.cost = obj.cost;
            tile.g = obj.costSoFar;
            tile.h = obj.h;
            tile.f = obj.f;
            tile.position = obj.position;
            return tile;
        }

        public override bool Equals(object obj)
        {
            return Equals((Tile)obj);
        }

        public bool Equals(Tile b)
        {
            if (position == b.position)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return (position.x << 5) + position.x ^ position.y;
        }
    }

    struct Tile 
    {
        public bool visted;
        public bool traverseable;
        public float cost;
        public float g;
        public float h;
        public float f;
        public Vector2Int position;

        public static implicit operator TileClass(Tile obj)
        {
            TileClass tile = new TileClass();
            tile.visted = obj.visted;
            tile.traverseable = obj.traverseable;
            tile.cost = obj.cost;
            tile.costSoFar = obj.g;
            tile.h = obj.h;
            tile.f = obj.f;
            tile.position = obj.position;
            return tile;
        }
        

    public override bool Equals(object obj)
        {
            return Equals((Tile)obj);
        }

        public bool Equals(Tile b)
        {
            if (position == b.position)
                return true;
            else
                return false;
        }

        public override int GetHashCode()
        {
            return (position.x << 5) + position.x ^ position.y;
        }
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
        Tile tempTile;
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

    List<Vector2Int> getPath(Vector2Int origin, Vector2Int target)
    {
        tileGrid.ForEach(v => v.ForEach(t => t.costSoFar = Mathf.Infinity));
        tileGrid.ForEach(v => v.ForEach(t => t.f = Mathf.Infinity));
        tileGrid.ForEach(v => v.ForEach(t => t.h = Vector2.Distance(t.position, target)));

        HashSet<TileClass> openList = new HashSet<TileClass>();
        HashSet<TileClass> closedList = new HashSet<TileClass>();

        TileClass currentTile = tileGrid[origin.y][origin.x];
        currentTile.visted = true;
        currentTile.costSoFar = 0;
        currentTile.f = currentTile.h;
        closedList.Add(currentTile);

        List<List<TileClass>> adjacentSquares = new List<List<TileClass>>();
        adjacentSquares.ForEach(v => v = new List<TileClass>());

        getNewAdjacentListSquare(adjacentSquares, origin);

        bool pathNotFound = true;
        adjacentSquaresToOpenList(currentTile, adjacentSquares, openList, origin, closedList);

        while (pathNotFound && openList.Count!=0)
        {
            Tile? minTile = null;
            foreach(Tile tile in openList)
            {
                if(minTile == null || minTile.Value.f < tile.f)
                {
                    minTile = tile;
                }
            }
            currentTile = (Tile)minTile;
            currentTile.visted = true;
            currentTile.costSoFar = 0;
            currentTile.f = currentTile.h;
            closedList.Add(currentTile);

        }
    }



    //BEGIN GENZO CODE

    class AStarData{
        public HashSet<TileClass> OpenList;
        public HashSet<TileClass> ClosedList;
        }
    private bool InclusiveBounds(int min, int max, int value){
        return (value >= min) && (value <= max);
        }
    private bool isValidTile(TileClass neighborTile) {

        if (neighborTile != null) return true; else return false;

        }
    private void updateOpenList(AStarData currentData, 
                                TileClass currentTile, 
                                List<List<TileClass>> AdjSquares){
        AdjSquares.ForEach(v => v.ForEach(u => { if (isValidTile(u)) updateAdjacentTiles(ref currentTile.position, currentTile, AdjSquares, currentData.OpenList, currentData.ClosedList, u.position.x, u.position.y); }));

        }

    //END GENZO CODE

        

    private void adjacentSquaresToOpenList(
    TileClass currentTile,
    List<List<TileClass>> adjacentSquares,
    Vector2Int position,
    HashSet<TileClass> openList,
    HashSet<TileClass> closedList)
    {
        TileClass TempTile;
        for (int row = -1; row <= 1; row++)
        {
            int verticleCellOffset = row + position.y;
            if (0 <= verticleCellOffset && verticleCellOffset < tileGrid.Count)
            {
                for (int col = -1; col <= 1; col++)
                {
                    int HorizontalCellOffset = col + position.x;
                    if (0 <= HorizontalCellOffset && HorizontalCellOffset < tileGrid[0].Count)
                    {
                        bool isNotCenter = !(row == 0 && col == 0);
                        if (isNotCenter)
                        {
                            
                            updateAdjacentTiles(ref position, currentTile, adjacentSquares, openList, closedList, row, col);
                        }
                    }
                }
            }
        }
    }

    void updateAdjacentTiles(
    ref Vector2Int position,
    TileClass currentTile,
    List<List<TileClass>> adjacentSquares,
    HashSet<TileClass> openList,
    HashSet<TileClass> closedList,
    int row,
    int column)
    {
        int rowOffset = row + 1;
        int columnOffset = column + 1;
        if (closedList.Contains(adjacentSquares[rowOffset][columnOffset]) == false && openList.Contains(adjacentSquares[rowOffset][columnOffset]) == false)
        {
            if (currentTile.costSoFar + adjacentSquares[rowOffset][columnOffset].cost < adjacentSquares[rowOffset][columnOffset].costSoFar)
            {
                adjacentSquares[rowOffset][columnOffset].costSoFar = currentTile.costSoFar + adjacentSquares[rowOffset][columnOffset].cost;
            }
            if (adjacentSquares[rowOffset][columnOffset].f > adjacentSquares[rowOffset][columnOffset].h + adjacentSquares[rowOffset][columnOffset].costSoFar)
            {
                adjacentSquares[rowOffset][columnOffset].f = adjacentSquares[rowOffset][columnOffset].h + adjacentSquares[rowOffset][columnOffset].costSoFar;

            }

            openList.Add(adjacentSquares[rowOffset][columnOffset]);
        }
    }

    Vector2Int getNext()
    {

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

    void getTile(out Tile tile, int x, int y)
    {
        tile = new Tile();
        Color tileData = heatMap.getPixel(x, y);
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
    }

    Vector2Int getMaxSizes()
    {
        return heatMap.getSize();
    }

    // Use this for initialization
    void Start () {
    }
    
    // Update is called once per frame
    void Update ()
    {


    }
}
