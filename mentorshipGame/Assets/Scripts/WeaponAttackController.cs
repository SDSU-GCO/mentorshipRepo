using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttackController : MonoBehaviour {

    [SerializeField]
    float timeToLive = 0.5f;//in seconds
    float originalTimeToLive;//in seconds
    public float AttackDelay = 0.75f;
    public bool attackingFriendlies = false;
    public int damage;
    public float speed = 7;
    new Collider collider = null;

    private void Awake()
    {
        originalTimeToLive = timeToLive;
        collider = GetComponent<Collider>();
        if (collider!=null && collider.isTrigger == false)
            collider = null;
    }

    // Update is called once per frame
    void Update () {
        
        timeToLive -= Time.deltaTime;
        if(collider != null && timeToLive< originalTimeToLive/2.0f)
        {
            collider.enabled = true;
        }
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
