using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class BossBoundary : MonoBehaviour {

    public GameObject Bosswall;
    
    public bool BossDefeated{get;private set;}

	// Use this for initialization
	void Start () {
        BossDefeated = false;
	    }

    private void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log(collision);
        if(BossDefeated == false){
            Bosswall.SetActive(true);
            }
        }

    }
