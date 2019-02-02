using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EnemyLogic : MonoBehaviour
{
    public bool isAgressive;
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

    public bool inRange { get; private set; }

    public static List<EnemyLogic> enemyLogics = new List<EnemyLogic>();

    private void OnEnable()
    {
        enemyLogics.Add(this);
    }
    private void OnDisable()
    {
        enemyLogics.Remove(this);
    }

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
            inRange = true;
            if (isAgressive == true)
            {
                GetComponent<Rigidbody2D>().velocity = -enVel;
            }
            Vector2.MoveTowards(enVel, target.transform.position, range);
            GetComponent<Animator>().SetBool("ChargeRange", true);

            //check if the enemy is in melee range
            if (range <= attackRange)
            {
                //after 2 seconds, attack
                secondsInRange = Mathf.Max(0, secondsInRange - Time.deltaTime);
//<<<<<<< HEAD
                GetComponent<Animator>().SetBool("MeleeRange", true);
                if (secondsInRange == 0 && attackCooldown == 0)
//=======
                if (secondsInRange == 0 && attackCooldown == 0 && isAgressive == true)
//>>>>>>> 3ead4a6132ba0772c4b8e9230eb9b75fb3fbaf17
                {
                    MeleeAttack();
                    attackCooldown = attackCooldownDefault;
                }
            }
            else
            {
                secondsInRange = Mathf.Min(secondsInRangeDefault, secondsInRange + Time.deltaTime);
                GetComponent<Animator>().SetBool("MeleeRange", false);
            }

        }
        //stop moving if out of range
        else
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            GetComponent<Animator>().SetBool("ChargeRange", false);
            GetComponent<Animator>().SetBool("MeleeRange", false);
            inRange = false;
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
