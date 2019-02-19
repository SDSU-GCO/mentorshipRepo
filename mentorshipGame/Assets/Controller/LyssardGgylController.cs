using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LyssardGgylController : SuperMonoBehaviour, IDeceaseable{

    public int Health = 100;

    public event EventHandler KillEvent;
    public event EventHandler InflictEvent;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void InflictDamage(int inDamage){
        if( (Health -= inDamage) <= 0 ) Kill();                       //Check if the reduction in health killed the player.
        if(InflictEvent != null) InflictEvent(this, EventArgs.Empty); //Announce that the object took damage
    }

    public void Kill(){
        if(KillEvent != null) KillEvent(this, EventArgs.Empty); //Announce that the object died
    }
}
