using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioEvent))]
[RequireComponent(typeof(Blink))]
[RequireComponent(typeof(DeleteTimer))]
public class AppleController : MonoBehaviour {

    private Blink       BlinkRef;
    private AudioEvent  AudioEventRef;
    public  DeleteTimer EventualDeleteRef;
    public  DeleteTimer OnHitDeleteRef;

    private event EventHandler StartEvent;
    private event EventHandler CollisionEvent;

	// Use this for initialization
	void Start () {
        BlinkRef       = GetComponent<Blink>();
        AudioEventRef  = GetComponent<AudioEvent>();

        StartEvent     += EventualDeleteRef.TimedDestroy;

        CollisionEvent += AudioEventRef.PlayHitSFX;
        CollisionEvent += BlinkRef.Pulse;
        CollisionEvent += OnHitDeleteRef.TimedDestroy;

        StartEvent.Invoke(this, EventArgs.Empty);
	}

    private void OnCollisionEnter2D(Collision2D collision) {
        CollisionEvent.Invoke(this, EventArgs.Empty);
    }

    // Update is called once per frame
    void Update () {
		
	}
}
