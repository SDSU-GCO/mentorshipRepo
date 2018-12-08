using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using cs;
using System;

public class EnemyLogic : MonoBehaviour
{

    Transform target;
    private float distanceToTarget;
    public float speed;
    public int health;
    public WeaponAttackController enemyMelee;
    public float attackCooldown = 2f;
    float attackCooldownDefault;
    public float secondsInRange = 2f;
    float secondsInRangeDefault;
    public float moveRange;
    public float attackRange;
    public float enemyRadius;
    new Rigidbody2D rigidbody2D;
    AStar2DPathfindingAgentController aStar2DPathfindingAgentController = null;

    private void Awake()
    {
        attackCooldownDefault = attackCooldown;
        secondsInRangeDefault = secondsInRange;
        rigidbody2D = GetComponent<Rigidbody2D>();
        aStar2DPathfindingAgentController = GetComponent<AStar2DPathfindingAgentController>();
    }
    Vector2 velocityOffsetTarget = new Vector2(0,0);
    float lerpProgress;

    void Update()
    {
        float minDistance = Mathf.Infinity;
        
        target = null;
        foreach (AllyLogic allyLogic in AllyLogic.AllyLogics)
        {
            //if (allyLogic.partyLeader == true)
                //target = allyLogic.transform;
            if (Vector2.Distance(transform.position, allyLogic.transform.position) < minDistance || target ==null)
            {
                minDistance = Vector2.Distance(transform.position, allyLogic.transform.position);
                target = allyLogic.transform;
            }
        }


        distanceToTarget = Vector2.Distance(transform.position, target.transform.position);
        
        attackCooldown = Mathf.Max(0, attackCooldown - Time.deltaTime);

        //detect and move to player if in range
        if (distanceToTarget <= moveRange)
        {
            if(hasLineOfSight(target, enemyRadius))
            {
                Vector2 enemyVelocity = new Vector2((transform.position.x - target.transform.position.x), (transform.position.y - target.transform.position.y));
                enemyVelocity = enemyVelocity.normalized * speed;
                rigidbody2D.velocity = -enemyVelocity;
            }
            else
            {
                aStar2DPathfindingAgentController.target = target;
                Vector2? temp = aStar2DPathfindingAgentController.getNextPoint();
                if(temp.HasValue)
                {
                    rigidbody2D.velocity = Vector2.MoveTowards(transform.position, temp.Value, 1).normalized *speed;
                }
            }

            //check if the enemy is in melee range
            if (distanceToTarget <= attackRange)
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
            velocityOffsetTarget = rigidbody2D.velocity;
            lerpProgress = 0;
        }
        //stop moving if out of range
        else
        {
            rigidbody2D.velocity = Vector2.Lerp(velocityOffsetTarget, Vector2.zero, lerpProgress);
            lerpProgress += (2f * Time.deltaTime);
        }


    }

    private bool hasLineOfSight(Transform target, float radius)
    {
        RaycastHit2D raycastHit2D = Physics2D.CircleCast(transform.position, radius, Vector2.MoveTowards(transform.position, target.position, 1).normalized);

        if (raycastHit2D.collider != null && raycastHit2D.collider.gameObject!=null &&  raycastHit2D.collider.gameObject == target.gameObject)
        {
            return true;
        }
        else
            return false;
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
