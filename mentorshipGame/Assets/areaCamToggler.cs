using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class areaCamToggler : MonoBehaviour {

    CinemachineVirtualCamera areaCam = null;

    private void Start()
    {
        areaCam = GetComponent<CinemachineVirtualCamera>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        areaCam.enabled = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        areaCam.enabled = false;
    }
}
