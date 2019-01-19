using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

public class areacamera : MonoBehaviour {
    CinemachineVirtualCamera attachedareacam;
    public static List<CinemachineVirtualCamera> cameralist = new List<CinemachineVirtualCamera>();
    public static CinemachineVirtualCamera playercam = null;
    private void OnDestroy()
    {
        if (cameralist != null && attachedareacam != null) 
        {
            cameralist.Remove(attachedareacam);
        }
    }
    // Use this for initialization
    void Awake () {
        attachedareacam = GetComponent<CinemachineVirtualCamera>();
        Debug.Assert(attachedareacam != null, "error attachedcamera null");
        cameralist.Add(attachedareacam);

	}


    private void OnTriggerEnter2D(Collider2D collision)
    {
        attachedareacam.enabled = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        attachedareacam.enabled = false;
    }
}
