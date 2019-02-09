using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSupervisor : MonoBehaviour {

    public GameObject firstimage;

	// Use this for initialization
	void Start () {
        Vector3 spwanposition = transform.position;
        spwanposition.z = spwanposition.z - 0.01f;
        if (firstimage == null)
            Debug.LogError("ya fuked up");
        Instantiate(firstimage, spwanposition, Quaternion.Euler(Vector3.zero));
	}
	
}
