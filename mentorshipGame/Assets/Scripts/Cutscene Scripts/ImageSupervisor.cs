using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSupervisor : MonoBehaviour {

    public GameObject firstimage;
    public Transform rootObject;

	// Use this for initialization
	void Start () {
        Vector3 spwanposition = transform.position;
        spwanposition.z = spwanposition.z - 0.01f;
        if (firstimage == null)
            Debug.LogError("ya fuked up");
        GameObject go = Instantiate(firstimage, spwanposition, Quaternion.Euler(Vector3.zero));
        go.transform.parent = rootObject;
	}
	
}
