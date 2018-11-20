using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyLogic : MonoBehaviour
{

    Transform target;
    private float range;
    public float speed;
    public int health;
    public WeaponAttackController enemyMelee;
    public float attackCooldown = 2f;
    float attackCooldownDefault;
    public float secondsInRange = 2f;
    float secondsInRangeDefault;
    public float moveRange;
    public float attackRange;


    private void Awake()
    {
        attackCooldownDefault = attackCooldown;
        secondsInRangeDefault = secondsInRange;
    }

    void Update()
    {
        float minDistance = Mathf.Infinity;
        


        target = null;
        foreach (AllyLogic allyLogic in AllyLogic.AllyLogics)
        {
            //if (allyLogic.partyLeader == true)
                //target = allyLogic.transform;
            if (Vector2.Distance(transform.position, allyLogic.transform.position) < minDistance)
            {
                minDistance = Vector2.Distance(transform.position, allyLogic.transform.position);
                target = allyLogic.transform;
            }
        }


        range = Vector2.Distance(transform.position, target.transform.position);
        
        attackCooldown = Mathf.Max(0, attackCooldown - Time.deltaTime);


        //detect and move to player if in range
        if (range <= moveRange)
        {
            Vector2 enVel = new Vector2((transform.position.x - target.transform.position.x), (transform.position.y - target.transform.position.y));
            enVel = enVel.normalized * speed;
            GetComponent<Rigidbody2D>().velocity = -enVel;
            Vector2.MoveTowards(enVel, target.transform.position, range);

            //check if the enemy is in melee range
            if (range <= attackRange)
            {
                //after 2 seconds, attack
                secondsInRange = Mathf.Max(0, secondsInRange - Time.deltaTime);
                if (secondsInRange == 0 && attackCooldown == 0)
                {
                    MeleeAttack();
                    attackCooldown = attackCooldownDefault;
                }
            }
            else
            {
                secondsInRange = Mathf.Min(secondsInRangeDefault, secondsInRange + Time.deltaTime);
            }

        }
        //stop moving if out of range
        else
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }


    }


    void MeleeAttack()
    {
        //gather offsets to determine the spawn location of the enemy's attack
        float offset = 1;
        Vector2 spawnOffset = ((Vector2)target.transform.position - (Vector2)transform.position).normalized * offset;
        Vector3 spawnLocation = transform.position + (Vector3)spawnOffset;
        spawnLocation.z = enemyMelee.transform.position.z;

        //rotate the attack towards the player
        Vector2 direction = spawnLocation - transform.position;
        float rotation = Mathf.Rad2Deg * (Mathf.Atan(direction.y / direction.x));
        rotation += -90;
        if (direction.x < 0)
        {
            rotation += 180;
        }

        //instantiate the attack
        GameObject child = Instantiate(enemyMelee.gameObject, spawnLocation, Quaternion.Euler(0, 0, rotation));
        child.transform.SetParent(transform);

        
    }
    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
            Destroy(gameObject);
    }
}
