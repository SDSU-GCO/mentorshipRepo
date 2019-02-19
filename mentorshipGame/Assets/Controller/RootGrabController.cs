using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioEvent))]
[RequireComponent(typeof(DeleteTimer))]
public class RootGrabController : SuperMonoBehaviour {

    private AudioEvent  AudioEventRef;
    private DeleteTimer EventualDeleteRef;

    private event EventHandler CollisionEvent;
    private event EventHandler StartEvent;

	// Use this for initialization
	void Start () {
        AudioEventRef     = GetComponent<AudioEvent>();
        EventualDeleteRef = GetComponent<DeleteTimer>();

        CollisionEvent += AudioEventRef.PlayHitSFX;
        StartEvent     += EventualDeleteRef.TimedDestroy;

        StartEvent.Invoke(this, EventArgs.Empty);
	    }

    private void OnCollisionEnter2D(Collision2D collision) {
        CollisionEvent.Invoke(this, EventArgs.Empty);
        }
}
