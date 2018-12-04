using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cinemachine;

public class playercamera : MonoBehaviour {
    CinemachineVirtualCamera playercam;
    
	// Use this for initialization
	void OnEnable () {
        StartCoroutine("Moncam");
        
	}
	IEnumerator Moncam()
    {
        do
        {
            yield return new WaitForEndOfFrame();
            if (areacamera.cameralist != null)
            {
                bool camEn = true;
                foreach (CinemachineVirtualCamera cvm in areacamera.cameralist)
                {
                    if (cvm.enabled == true)
                        camEn = false;
                }
                playercam.enabled = camEn;
            }
        } while (enabled == true);
    }
	// Update is called once per frame
	void Awake () {
        playercam = GetComponent<CinemachineVirtualCamera>();
        Debug.Assert(playercam != null, "error playercamera null");
        playercam.enabled = true;
        areacamera.playercam = playercam;
    }
}
