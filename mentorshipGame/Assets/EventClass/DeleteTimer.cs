using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DeleteTimer : MonoBehaviour {
    
    public float DeleteTime = 1;

    private IEnumerator DeleteTimerCorroutine(float time){
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
        }
    public void TimedDestroy(object sender, EventArgs e){
        StartCoroutine(DeleteTimerCorroutine(DeleteTime));
        }
}
