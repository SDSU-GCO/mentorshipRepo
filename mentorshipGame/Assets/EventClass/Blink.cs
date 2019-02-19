using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(SpriteRenderer))]
public class Blink : MonoBehaviour {

    private SpriteRenderer rendererRef;
    private SuperMonoBehaviour SmbRef;
    private bool flickering = false;
    public float duration = 1;
    public float onPulse  = 0.5f;
    public float offPulse = 0.5f;

	// Use this for initialization
	void Start () {
		rendererRef = GetComponent<SpriteRenderer>();
	    }

    /// <summary>
    /// Turn renderer on and off for each defined duration. 
    /// </summary>
    /// <param name="onPulse">Length of time to render</param>
    /// <param name="offPulse">Length of time to not render</param>
    /// <returns></returns>
    private IEnumerator Flicker(float onPulse, float offPulse){
        flickering = true;
        yield return new WaitForSeconds(onPulse); //Duration to keep on
        rendererRef.enabled = false;
        yield return new WaitForSeconds(offPulse); //Duration to keep off
        rendererRef.enabled = true;
        flickering = false;
        }

    /// <summary>
    /// Continuoutly flicker renderer.
    /// </summary>
    /// <param name="duration">Length of time to flicker</param>
    /// <param name="onPulse">Length of time to render</param>
    /// <param name="offPulse">Length of time to not render</param>
    /// <returns></returns>
    private IEnumerator PulseRoutine(float duration, float onPulse, float offPulse){
        float StartTime = Time.time;
        float EndTime = StartTime + duration;
        while(Time.time < EndTime){
            if(flickering == false) StartCoroutine(Flicker(onPulse, offPulse));
            yield return new WaitForEndOfFrame();
            }
        yield return new WaitForEndOfFrame();
        }
    
    /// <summary>
    /// Continuously flicker renderer.
    /// </summary>
    /// <param name="sender">Sender object</param>
    /// <param name="e">Event Arguments</param>
    public void Pulse(object sender, EventArgs e){
        StartCoroutine( PulseRoutine(duration, onPulse, offPulse) );
        }

    }
