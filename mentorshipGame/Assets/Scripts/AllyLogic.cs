using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AllyLogic : MonoBehaviour, IGameObjectAddedToHierarchy
{
    private bool IsInHierarchy = false;
    public WeaponAttackController weaponAttack;
    public bool partyLeader;
    public static AllyLogic partyLeaderObject;
    [SerializeField]
    float MaxRepulsionForce;
    [SerializeField]
    float weaponCoolDownInSeconds;
    float weaponCoolDownInSecondsDefault;
    [SerializeField]
    float rangedCoolDownInSeconds;
    float rangedCoolDownInSecondsDefault;
    public float aggroRange;
    public float meleeRange = 1.5f;
    public int playerHealth;
    public int damage;
    public int charachterID;
    public float repulsionForceRange;
    public float moveSpeed;
    public WeaponAttackController rangedAttack;
    //Vector2 velocity = new Vector2(0,0);
    //Vector2 forcesFromAllies = new Vector2(0, 0);
    public float followDistanceMax;
    public float followDistanceMin;
    Rigidbody2D my2DRigidbody;
    //[SerializeField]
    //float forceOfNearbyAllies = 1;

    [HideInInspector]
    public static List<AllyLogic> AllyLogics = new List<AllyLogic>();
    

    private void Awake()
    {
        weaponCoolDownInSeconds = weaponAttack.AttackDelay;
        weaponCoolDownInSecondsDefault = weaponCoolDownInSeconds;
        rangedCoolDownInSeconds = rangedAttack.AttackDelay;
        rangedCoolDownInSecondsDefault = rangedCoolDownInSeconds;
        my2DRigidbody = GetComponent<Rigidbody2D>();

    }
    private void OnEnable()
    {
        AllyLogics.Add(this);
        
    }
    private void OnDisable()
    {
        AllyLogics.Remove(this);
    }

    float rotationVariable = 0;
    public float rotationSpeed = 5;
    void Update ()
    {

        if (partyLeader == true)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");
            
            Vector2 verticalComponent = ((Vector2)transform.up) * moveVertical;
            Vector2 horizontalComponent = ((Vector2)transform.right) * moveHorizontal;
            Vector2 calculatedVelocity = verticalComponent + horizontalComponent;
            my2DRigidbody.velocity = calculatedVelocity * moveSpeed;


            weaponCoolDownInSeconds = Mathf.Max(0, weaponCoolDownInSeconds - Time.deltaTime);
            rangedCoolDownInSeconds = Mathf.Max(0, rangedCoolDownInSeconds - Time.deltaTime);
            if (Input.GetMouseButtonDown(0) && weaponCoolDownInSeconds == 0)
            {
                FireAttack();
                weaponCoolDownInSeconds = weaponCoolDownInSecondsDefault;
            }
            if (Input.GetMouseButtonDown(1) && rangedCoolDownInSeconds == 0)
            {
                FireRangedAttack();
                rangedCoolDownInSeconds = rangedCoolDownInSecondsDefault;
            }
            //Rotate Screen
            if (Input.GetKey("q"))
            {

                rotationVariable = rotationVariable + (rotationSpeed);
                if (rotationVariable > 360)
                {
                    rotationVariable = rotationVariable - 360;
                }
                transform.rotation = Quaternion.Euler(0, 0, rotationVariable);

            }
            if (Input.GetKey("e"))
            {

                rotationVariable = rotationVariable - (rotationSpeed);
                if (rotationVariable < 0)
                {
                    rotationVariable = rotationVariable + 360;
                }
                transform.rotation = Quaternion.Euler(0, 0, rotationVariable);
            }
        }
        else
        {
            //applies to allies
            
        }
        foreach (AllyLogic ally in AllyLogics)
        {
            if(ally != this)
            {
                EnemyLogic closestEnemy = null;
                float absNearest=-1;
                foreach(EnemyLogic enemyLogic in EnemyLogic.enemyLogics)
                {
                    float nearest = Vector2.Distance(transform.position, enemyLogic.transform.position);
                    if(absNearest==-1 || nearest<absNearest)
                    {
                        absNearest = nearest;
                        closestEnemy = enemyLogic;
                    }
                }
                
                if((closestEnemy == null) || Vector2.Distance(transform.position, closestEnemy.transform.position) > aggroRange)
                {
                    if ((ally != this && (Vector2.Distance(GetPartyLeader().transform.position, transform.position) > repulsionDistance)))
                    {
                        Vector2 forceTowardLeader = GetPartyLeader().transform.position - transform.position;
                        my2DRigidbody.velocity = forceTowardLeader;
                    }
                }
                else if((closestEnemy != null) && this != GetPartyLeader())
                {

                    Vector2 forceTowardEnemy = closestEnemy.transform.position - transform.position;
                    my2DRigidbody.velocity = forceTowardEnemy;

                    if (Vector2.Distance(GetPartyLeader().transform.position, transform.position) < meleeRange)
                    {
                        if (weaponCoolDownInSeconds <= 0)
                        {
                            weaponCoolDownInSeconds = weaponCoolDownInSecondsDefault * 2;
                            //gather offsets to determine the spawn location of the enemy's attack
                            float offset = 1;
                            Vector2 spawnOffset = ((Vector2)closestEnemy.transform.position - (Vector2)transform.position).normalized * offset;
                            Vector3 spawnLocation = transform.position + (Vector3)spawnOffset;
                            spawnLocation.z = weaponAttack.transform.position.z;

                            //rotate the attack towards the player
                            Vector2 direction = spawnLocation - transform.position;
                            float rotation = Mathf.Rad2Deg * (Mathf.Atan(direction.y / direction.x));
                            rotation += -90;
                            if (direction.x < 0)
                            {
                                rotation += 180;
                            }

                            GameObject child = Instantiate(weaponAttack.gameObject, spawnLocation, Quaternion.Euler(0, 0, rotation));
                            child.transform.SetParent(transform);
                        }
                        weaponCoolDownInSeconds = Mathf.Max(0, weaponCoolDownInSeconds - Time.deltaTime);
                    }
                    else
                    {
                        if (rangedCoolDownInSeconds <= 0)
                        {
                            rangedCoolDownInSeconds = rangedCoolDownInSecondsDefault * 2;
                            //gather offsets to determine the spawn location of the enemy's attack
                            float offset = 1;
                            Vector2 spawnOffset = ((Vector2)closestEnemy.transform.position - (Vector2)transform.position).normalized * offset;
                            Vector3 spawnLocation = transform.position + (Vector3)spawnOffset;
                            spawnLocation.z = weaponAttack.transform.position.z;

                            //rotate the attack towards the enemy
                            Vector2 direction = spawnLocation - transform.position;
                            float rotation = Mathf.Rad2Deg * (Mathf.Atan(direction.y / direction.x));
                            rotation += -90;
                            if (direction.x < 0)
                            {
                                rotation += 180;
                            }

                            GameObject childInstance = Instantiate(rangedAttack.gameObject, spawnLocation, Quaternion.Euler(0, 0, rotation));
                            childInstance.GetComponent<Rigidbody2D>().velocity = rangedAttack.speed * direction.normalized;
                        }
                        rangedCoolDownInSeconds = Mathf.Max(0, rangedCoolDownInSeconds - Time.deltaTime);
                    }
                }


                if ((ally != this && (Vector2.Distance(ally.transform.position, transform.position) < repulsionForceRange)))
                {
                    Vector2 difference = ally.transform.position - transform.position;
                    float force = (-MaxRepulsionForce/aggroRange)* Vector2.Distance(ally.transform.position, transform.position) + MaxRepulsionForce;

                    my2DRigidbody.AddForce(-force * difference.normalized);
                }
            }
        }
    }

    public float offset = 1.5f;
    //CircleCollider2D circleCollider2D = null;

    [SerializeField]
    float repulsionDistance;

    public static AllyLogic  GetPartyLeaderGlobal()
    {

        AllyLogic partylead = null;
        foreach (AllyLogic allyLogic in AllyLogics)
        {
            if (allyLogic.partyLeader == true)
                partylead = allyLogic;
        }
        return partylead;
    }

    public AllyLogic GetPartyLeader()
    {
        
        AllyLogic partylead = null;
        foreach(AllyLogic allyLogic in AllyLogics)
        {
            if (allyLogic.partyLeader == true)
                partylead = allyLogic;
        }
        return partylead;
    }

    void FireAttack()
    {
        {
            float offset = 1;
            Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 spawnOffset = (mouseWorldPosition - (Vector2)transform.position).normalized * offset;
            Vector3 spawnLocation = transform.position + (Vector3)spawnOffset;
            spawnLocation.z = weaponAttack.transform.position.z;


            Vector2 direction = spawnLocation - transform.position;
            float rotation = Mathf.Rad2Deg * (Mathf.Atan(direction.y / direction.x));
            rotation += -90;
            if(direction.x<0)
            {
                rotation += 180;
            }
            
            GameObject child = Instantiate(weaponAttack.gameObject, spawnLocation, Quaternion.Euler(0, 0, rotation));
            child.transform.SetParent(transform);


        }
    }

    void FireRangedAttack()
    {
        
        if (rangedCoolDownInSeconds == 0)
        {
                

            Vector3 mouseScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);


            Vector2 mouseposition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
            mouseposition = (mouseposition - (Vector2)transform.position).normalized * offset;



            Vector3 shoot = (new Vector3(mouseposition.x, mouseposition.y, 0) - transform.position).normalized;
            GameObject childInstance = Instantiate(rangedAttack.gameObject, mouseposition + (Vector2)transform.position, transform.rotation);
            childInstance.GetComponent<Rigidbody2D>().velocity = rangedAttack.speed * mouseposition.normalized;

            rangedCoolDownInSeconds = rangedCoolDownInSecondsDefault;
                
        }
        
    }
    [SerializeField]
    HeartGroup heartGroup;

    //update player HP & hearts upon damage and run checks to keep HP within max and min bounds
    public void TakeDamage(int amount)
    {
        playerHealth -= amount;
        
        Debug.Assert(heartGroup != null, "Error: heartGroup cannot be null on " + this);
        if (partyLeader == true)
            heartGroup.updateHeart(playerHealth);

        //Death message and prevent player from going to negative HP
        if (playerHealth <= 0 && partyLeader == true)
        {
            Debug.Log("Game Over");
            playerHealth = 0;
        }
        //prevent player from having more than their max HP
        else if (playerHealth >= 3 && partyLeader == true)
            playerHealth = 3;
    }

    public bool IsAdded()
    {
        return IsInHierarchy;
    }
    public void AddToHierarchy()
    {
        IsInHierarchy = true;
        heartGroup = FindObjectOfType<HeartGroup>();
    }
}
