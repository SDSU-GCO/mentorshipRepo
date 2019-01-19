using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour {
    public List<AllyLogic> setCharachters = new List<AllyLogic>();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        for (int key = 0; key<setCharachters.Count;key++)
        if (Input.GetKeyDown((key + 1).ToString()))
        {
                foreach(AllyLogic ally in setCharachters)
                {
                    ally.partyLeader = false;
                }
                setCharachters[key].partyLeader = true;
                Camera.main.transform.parent = setCharachters[key].transform;
                Vector3 temp = new Vector3(setCharachters[key].transform.position.x, setCharachters[key].transform.position.y, Camera.main.transform.position.z);
                Camera.main.transform.position = temp;
                Camera.main.transform.rotation = setCharachters[key].transform.rotation;
            }
    }
}
