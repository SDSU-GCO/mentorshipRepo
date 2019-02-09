using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttackController : MonoBehaviour {

    [SerializeField]
    float timeToLive = 0.5f;//in seconds
    public float AttackDelay = 0.75f;
    public bool attackingFriendlies = false;
    public int damage;
    public float speed = 7;
	
	// Update is called once per frame
	void Update () {
        timeToLive -= Time.deltaTime;
        if (timeToLive <= 0)
            Destroy(gameObject);

	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "ally" && attackingFriendlies == true)
        {
            collision.gameObject.GetComponent<AllyLogic>().TakeDamage(damage);
        }
        else if (collision.gameObject.tag == "enemy" && attackingFriendlies == false)
        {
            collision.gameObject.GetComponent<EnemyLogic>().TakeDamage(damage);
        }
        Destroy(gameObject);
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "ally" && attackingFriendlies == true)
        {
            collision.gameObject.GetComponent<AllyLogic>().TakeDamage(damage);
        }
        else if (collision.tag == "enemy" && attackingFriendlies == false)
        {
            collision.gameObject.GetComponent<EnemyLogic>().TakeDamage(damage);
        }
    }

}
