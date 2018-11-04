using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

public class TileMapSceneSupervisor : MonoBehaviour{

    private void Reset()
    {
        TileMapController[] tileMapControllers = FindObjectsOfType<TileMapController>();
        foreach(TileMapController tileMapController in tileMapControllers)
        {
            if(tileMapController.tileMapSupervisor == null)
            {
                tileMapController.tileMapSupervisor = this;
            }
        }

        AStar2DPathfindingAgentController[] aStar2DPathfindingAgentControllers = FindObjectsOfType<AStar2DPathfindingAgentController>();
        foreach (AStar2DPathfindingAgentController aStar2DPathfindingAgentController in aStar2DPathfindingAgentControllers)
        {
            if (aStar2DPathfindingAgentController.tileMapSupervisor == null)
            {
                aStar2DPathfindingAgentController.tileMapSupervisor = this;
            }
        }
    }

    public List<TileMapController> tileMapControllers = new List<TileMapController>();

    public TileMapController GetTileMapController(Transform transform)
    {
        TileMapController currentTileMapController = null;

        foreach(TileMapController tileMapController in tileMapControllers.TakeWhile(x=> (currentTileMapController == null )))
        {
            TilemapCollider2D tilemapCollider2D = tileMapController.GetComponent<TilemapCollider2D>();
            if(tilemapCollider2D.bounds.Contains(transform.position))
            {
                currentTileMapController = tileMapController;
            }
        }

        return currentTileMapController;
    }
}
