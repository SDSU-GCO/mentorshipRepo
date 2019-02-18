using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(EnemyLogic))]
public class HamsterStates : MonoBehaviour {

    private SpriteRenderer spriteRenderer;
    private EnemyLogic enemyLogicRef;

    public Sprite SleepingSprite;
    public Sprite AngrySprite;

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
        enemyLogicRef   = GetComponent<EnemyLogic>();
	    }
	    
	// Update is called once per frame
	void Update () {

        if( enemyLogicRef.inRange == true){
            spriteRenderer.sprite = AngrySprite;
            }else{
            spriteRenderer.sprite = SleepingSprite;
            }

	    }
    }
