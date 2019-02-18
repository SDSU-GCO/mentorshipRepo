using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSupervisor : MonoBehaviour {

    public GameObject firstimage;
    public Transform rootObject;

	// Use this for initialization
	void Start () {
        Vector3 spwanposition = Vector3.zero;
        if (firstimage == null)
            Debug.LogError("ya fuked up");
        GameObject go = Instantiate(firstimage, spwanposition, Quaternion.Euler(Vector3.zero));
        RectTransform newTransform = go.GetComponent<RectTransform>();
        newTransform.SetParent(rootObject);
        newTransform.localPosition = (spwanposition);
        newTransform.localScale = Vector3.one;
        newTransform.sizeDelta = new Vector2(0, 0);
    }
	
}
