using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStar2DPathfindingAgentController : MonoBehaviour {

    public GameObject TargetObject;
    public TileMapController tileMapController;
    public TileMapSceneSupervisor tileMapSupervisor = null;

    private void Reset()
    {
        tileMapSupervisor = FindObjectOfType<TileMapSceneSupervisor>();
    }
    private void Awake()
    {
        tileMapController = tileMapSupervisor.GetTileMapController(transform);
    }

    Vector2 GotoPoint(Transform target)
    {
        Transform gridTransform = tileMapController.transform;
        Debug.Assert(gridTransform.rotation == Quaternion.Euler(0, 0, 0), "Error: The tilemap should never be rotated!");

        Vector3 tileSize = tileMapController.GetComponent<Tilemap>().cellSize;
        /*

        Vector2 nextPosition;
        Vector2Int originTile = transform.position;
        Vector2Int targetTile = target.position;

        tileMapController.getNextTile(originTile, targetTile);
        */
        return new Vector2();
    }
}
