using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(DeleteTimer))]
public class Damage : MonoBehaviour {

    public int DamageValue = 1;

    public void Start() {
        }

    private void OnCollisionEnter2D(Collision2D collision) {
        MonoBehaviour inMono = collision.gameObject.GetComponent<SuperMonoBehaviour>();

        if( inMono is IDamageable ) ((IDamageable)inMono).InflictDamage(DamageValue);

        }

    }
