using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace cs
{
    /// <summary>
    /// This should be atatched to an object that needs to navigate through a tilemap to a target point.
    /// </summary>
    public class AStar2DPathfindingAgentController : MonoBehaviour
    {
        public TileMapSceneSupervisor tileMapSupervisor;
        [SerializeField]
        TileMapController tileMapController;
        Tilemap tilemap;
        public Transform target;
        Transform oldTarget;
        List<Vector3Int> nextPoints;
        Vector3Int? nextPoint;
        bool wasRunThisFrame;
        bool wasRunThisFramePoint;

        private void Reset()
        {
            tileMapSupervisor = FindObjectOfType<TileMapSceneSupervisor>();
            tileMapController = tileMapSupervisor.GetTileMapController(transform);
            tilemap = tileMapController.GetComponent<Tilemap>();
        }
        
        
        /// <summary>
        /// gets a path of points that leads to the target.
        /// </summary>
        /// <returns></returns>
        public List<Vector2> getPath()
        {
            if(!wasRunThisFrame || target != oldTarget)
            {
                oldTarget = target;
                wasRunThisFrame = true;
                Transform gridTransform = tileMapController.transform;
                Debug.Assert(gridTransform.rotation == Quaternion.Euler(0, 0, 0), "Error: The tilemap should never be rotated!");
                Debug.Assert(target != null, "Error: Target can not be null!");

                attemptPopulate();

                Debug.Assert(tileMapController != null, "Error: Can't get reference to tileMapController!");
                Debug.Assert(tilemap != null, "Error: Can't get reference to tilemap!");

                Vector3Int originTile = tilemap.WorldToCell(transform.position);
                Vector3Int targetTile = tilemap.WorldToCell(target.transform.position);

                Debug.Assert(tilemap.cellBounds.Contains(originTile) && tilemap.cellBounds.Contains(targetTile), "Error: Path goes out of bounds!");

                nextPoints = tileMapController.getPath(originTile, targetTile);
            }
            
            List<Vector2> nextPoints2D = new List<Vector2>();
            

            if (nextPoints == null)
            {
                nextPoints2D = null;
            }
            else
            {
                foreach(Vector3Int vector3Int in nextPoints)
                {
                    nextPoints2D.Add(tilemap.CellToWorld(vector3Int));
                }
            }

            return nextPoints2D;
        }

        private void attemptPopulate()
        {
            if (tileMapSupervisor == null)
            {
                tileMapSupervisor = FindObjectOfType<TileMapSceneSupervisor>();
                Debug.Assert(tileMapSupervisor != null, "Error: tileMapSupervisor==null in " + this);
            }

            if (tileMapController == null)
            {
                tileMapController = tileMapSupervisor.GetTileMapController(transform);
                Debug.Assert(tileMapController != null, "Error: tileMapController==null in " + this);
            }

            if (tilemap == null)
            {
                tilemap = tileMapController.GetComponent<Tilemap>();
                Debug.Assert(tilemap != null, "Error: tilemap==null in " + this);
            }
        }


        /// <summary>
        /// gets the next point on a path to the target
        /// </summary>
        /// <returns></returns>
        public Vector2? getNextPoint()
        {
            attemptPopulate();
            if (!wasRunThisFramePoint || target != oldTarget)
            {
                oldTarget = target;
                wasRunThisFrame = true;
                Transform gridTransform = tileMapController.transform;
                Debug.Assert(gridTransform.rotation == Quaternion.Euler(0, 0, 0), "Error: The tilemap should never be rotated!");
                Debug.Assert(target != null, "Error: Target can not be null!");

                Vector3Int originTile = new Vector3Int();
                Vector3Int targetTile = new Vector3Int();
                originTile = tilemap.WorldToCell(transform.position);
                targetTile = tilemap.WorldToCell(target.transform.position);

                Debug.Assert(tilemap.cellBounds.Contains(originTile) && tilemap.cellBounds.Contains(targetTile), "Error: Path goes out of bounds!");

                nextPoint = tileMapController.getNextTile(originTile, targetTile);
            }

            Vector2? nextPoint2D = new Vector2?();

            if (!nextPoint.HasValue)
            {
                nextPoint2D = null;
            }
            else
            {
                nextPoint2D = new Vector2?(tilemap.CellToWorld((Vector3Int)nextPoint));
            }

            return nextPoint2D;
        }

        public bool pointIsInGrid(Vector3 point)
        {
            return tilemap.cellBounds.Contains(tilemap.WorldToCell(point));
        }

        private void LateUpdate()
        {
            wasRunThisFrame = false;
        }
    }
}