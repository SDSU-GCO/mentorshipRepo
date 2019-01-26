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
        AllyLogic allyLogic = collision.gameObject.GetComponent<AllyLogic>();

        if (allyLogic != null && allyLogic.partyLeader == true)
        {
            areaCam.enabled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        AllyLogic allyLogic = collision.gameObject.GetComponent<AllyLogic>();

        if (allyLogic != null && allyLogic.partyLeader == true)
        {
            areaCam.enabled = false;
        }
    }
}
