using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyLogic : MonoBehaviour
{
    private GameObject laserBeam;
    public bool isPlant;
    public bool isAgressive;
    public bool canMove;
    Transform target;
    private float range;
    public float speed;
    public int health;
    public WeaponAttackController enemyMelee;
    public float attackCooldown = 2f;
    float attackCooldownDefault;
    public float rangedAttackCooldown = 2f;
    float rangedAttackCooldownDefault;
    public float secondsInRange = 2f;
    float secondsInRangeDefault;
    public float moveRange;
    public float attackRange;
    public float rangedRange;
    public LineRenderer lrRef;



    private void Awake()
    {
        attackCooldownDefault = attackCooldown;
        rangedAttackCooldownDefault = rangedAttackCooldown;
        secondsInRangeDefault = secondsInRange;
        lrRef = GetComponent<LineRenderer>();
    }

    void Update()
    {

        //rotate the enemy towards the player
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

             //set enemy direction
        
                  Vector3 enemyDirection = target.position - transform.position;
                  float enemyRotation = Mathf.Rad2Deg * (Mathf.Atan(enemyDirection.y / enemyDirection.x));
                  enemyRotation += -90;
                  if (enemyDirection.x < 0)
                  {
                      enemyRotation += 180;
                  }
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, enemyRotation));
                
       

            }
        }


        range = Vector2.Distance(transform.position, target.transform.position);
        
        attackCooldown = Mathf.Max(0, attackCooldown - Time.deltaTime);


        //detect and move to player if in range
        if (range <= moveRange)
        {
            Vector2 enVel = new Vector2((transform.position.x - target.transform.position.x), (transform.position.y - target.transform.position.y));
            enVel = enVel.normalized * speed;
            if (isAgressive && canMove == true)
            {
                GetComponent<Rigidbody2D>().velocity = -enVel;
            }
            Vector2.MoveTowards(enVel, target.transform.position, range);

            //check if the enemy is in melee range
            if (range <= attackRange)
            {
                //after 2 seconds, attack
                secondsInRange = Mathf.Max(0, secondsInRange - Time.deltaTime);
                if (secondsInRange == 0 && attackCooldown == 0 && isAgressive == true)
                {
                    MeleeAttack();
                    attackCooldown = attackCooldownDefault;
                }
            }
            else
            {
                secondsInRange = Mathf.Min(secondsInRangeDefault, secondsInRange + Time.deltaTime);
            }


            if (range <= rangedRange)
            {
                //after 2 seconds, attack
                secondsInRange = Mathf.Max(0, secondsInRange - Time.deltaTime);
                if (secondsInRange == 0 && rangedAttackCooldown == 0 && isAgressive == true)
                {
                    rangedAttack();
                    rangedAttackCooldown = rangedAttackCooldownDefault;
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
    void rangedAttack()
    {
        //GameObject bigChungus = Instantiate(laserBeam);
        //LineRenderer theBigOne = bigChungus.GetComponent<LineRenderer>();

        StartCoroutine(LaserTravel(3f));
    }
    IEnumerator LaserTravel(float duration)
    {
        float endTime = Time.time + duration;
        Vector2 offSetVector = transform.position - target.position;
        RaycastHit2D results;
        LayerMask layerMask = LayerMask.NameToLayer("Enemy");
        layerMask = ~layerMask;
        results = Physics2D.Raycast(transform.position, offSetVector, Mathf.Infinity, layerMask);

        //damage calculation goes here

        do
        {

            lrRef.SetPosition(0, new Vector3(transform.position.x, transform.position.y, -1f));
            lrRef.SetPosition(1, new Vector3(results.point.x, results.point.y, -1f));
            yield return null;

            results = Physics2D.Raycast(transform.position, offSetVector, Mathf.Infinity, layerMask);
        } while (Time.time < endTime);
    }

    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health <= 0)
            Destroy(gameObject);
        isAgressive = true;
    }
}
