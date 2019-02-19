using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class AudioEvent : MonoBehaviour {

    public AudioClip HitSFX;
    public float Volume;
    private AudioSource AudioSourceRef;
    
	// Use this for initialization
	void Start () {
		AudioSourceRef = GetComponent<AudioSource>();
	}

    public void PlayHitSFX(object sender, EventArgs e){
        AudioSourceRef.volume = Volume;
        AudioSourceRef.PlayOneShot(HitSFX);
    }
}
