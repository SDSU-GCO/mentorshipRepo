using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar2DPathfindingAgentController : MonoBehaviour {


    public TileMapSceneSupervisor tileMapSupervisor = null;

    private void Reset()
    {
        tileMapSupervisor = FindObjectOfType<TileMapSceneSupervisor>();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
