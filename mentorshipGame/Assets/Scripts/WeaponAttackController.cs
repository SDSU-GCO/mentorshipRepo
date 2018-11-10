using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttackController : MonoBehaviour {

    [SerializeField]
    float timeToLive = 0.5f;//in seconds
    public float attackDelay = 0.75f;
    
	
	// Update is called once per frame
	void Update () {
        timeToLive -= Time.deltaTime;
        if (timeToLive <= 0)
            Destroy(gameObject);
	}
}
