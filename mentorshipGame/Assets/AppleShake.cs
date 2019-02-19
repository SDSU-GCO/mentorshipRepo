using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleShake : MonoBehaviour {

    public float ShakeCooldown = 1;
    public GameObject NormalApple;
    public GameObject PoisonApple;
    public Transform LeftEdge;
    public Transform RightEdge;
    public float strength;

    private bool dispensing = false;

    private void DropApple(){
        GameObject  Apple      = Instantiate( (Random.Range(0,10)%10 == 0) ? NormalApple : PoisonApple );
        Rigidbody2D AppleRbRef = Apple.GetComponent<Rigidbody2D>();
        
        Apple.transform.position = transform.position;
        AppleRbRef.velocity = new Vector2(Mathf.Lerp(LeftEdge.localPosition.x, RightEdge.localPosition.x, Random.value), -1).normalized * strength;
        }
    
    private IEnumerator TriggerApples(int count, float deltatime){
        dispensing = true;
        for(int i=count; i>0; i--){
            DropApple();
            yield return new WaitForSeconds(deltatime);
            }
        dispensing = false;
        }

    public void DropApples(int count, float deltatime){
        if(dispensing == false) StartCoroutine(TriggerApples(count, deltatime));
        }

    public void Update() {
        DropApples(10, 0.7f);
        }

    }
