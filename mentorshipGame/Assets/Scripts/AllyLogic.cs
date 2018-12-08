using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using WeaponSupervisor;

public class AllyLogic : MonoBehaviour
{
    //bool PlayerAttack = false;
    //bool SpAvailable = false;
    public WeaponAttackController weaponAttack;
    public bool partyLeader;
    float weaponCoolDownInSeconds;
    float weaponCoolDownInSecondsDefault;
    float rangedCoolDownInSeconds;
    float rangedCoolDownInSecondsDefault;
    public int playerHealth;
    public int damage;
    public float moveSpeed;
    public WeaponAttackController rangedAttack;
    Vector2 velocity = new Vector2(0,0);
    public float followDistanceMax;
    public float followDistanceMin;
    Rigidbody2D my2DRigidbody;
    List<AllyLogic> nearbyAllies = new List<AllyLogic>();
    Vector2 forcesFromAllies = new Vector2(0, 0);
    [SerializeField]
    float forceOfNearbyAllies = 1;

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

    //AllyLogics.Add
    float rotationVariable = 0;
    public float rotationSpeed = 5;
    void Update ()
    {

        if (partyLeader == true)
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            velocity.x = moveHorizontal * moveSpeed;
            velocity.y = moveVertical * moveSpeed;
            my2DRigidbody.velocity = velocity;


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
            /*AllyLogic partyLeader = GetPartyLeader();
            if (partyLeader != null)
            {

                float distance = Vector2.Distance(partyLeader.transform.position, transform.position);
                if (canSeeTargetAlly(partyLeader))
                {
                    if (distance < followDistanceMin)
                    {
                        //back off
                        my2DRigidbody.velocity = (((Vector2)transform.position) - ((Vector2)partyLeader.transform.position)).normalized * moveSpeed;
                    }
                    else if (distance < followDistanceMax)
                    {
                        //stay still
                    }
                    else
                    {
                        //move closer
                        my2DRigidbody.velocity = (((Vector2)partyLeader.transform.position) - ((Vector2)transform.position)).normalized * moveSpeed;
                    }
                }
                else
                {
                    //path to ally
                    Vector2 nextPoint = getNextPoint;
                    my2DRigidbody.velocity = (nextPoint - (Vector2)transform.position).normalized * moveSpeed;
                }
            }
        }

        getNearbyAllies();
        foreach(AllyLogic ally in AllyLogics)
        {
            Vector2 tempForce = new Vector2(forceOfNearbyAllies, forceOfNearbyAllies);
            tempForce.Scale( ((Vector2)transform.position - (Vector2)ally.transform.position));
            forcesFromAllies += tempForce;
        }*/

        //getComponent.characterManager.Character.SpecialAttack;

        /**if (SpAvailable == true && Input.GetKeyDown("space"))
        {
            fireSpecial();
        }

        bool superfriendshipbeam = false;
        bool PhilSpecial = getComponent.characterManager.Philif.SpecialAttack;
        bool FelSpecial = getComponent.characterManager.Felicia.SpecialAttack;
        bool VerSpecial = getComponent.characterManager.Veronica.SpecialAttack;
        bool TaySpecial = getComponent.characterManager.Taylor.SpecialAttack;
    
        foreach (AllyLogic allyLogic in AllyLogic)
        {
            SuperFriendshipBeam();
        }
        **/
	}

    /*private void getNearbyAllies()
    {
        nearbyAllies.Clear();
        foreach(AllyLogic ally in AllyLogics)
        {
            if(ally!=this && Vector2.Distance(transform.position, ally.transform.position) <= repulsionDistance)
            {
                nearbyAllies.Add(ally);
            }
        }
        return;
    */}

    public float offset = 1.5f;
    //GameObject gameObjectTarget = new GameObject();
    CircleCollider2D circleCollider2D = null;

    [SerializeField]
    float repulsionDistance;

    /*private bool canSeeTargetAlly(AllyLogic ally)
    {
        bool canSeeTarget = false;
        gameObjectTarget.SetActive(true);
        
        CircleCollider2D temp = gameObjectTarget.GetComponent<CircleCollider2D>();
        if (temp == null)
            circleCollider2D = gameObjectTarget.AddComponent<CircleCollider2D>();
        else
            circleCollider2D = temp;
        
        circleCollider2D.enabled = true;
        circleCollider2D.radius = 0.05f;
        circleCollider2D.offset = offset;

        gameObjectTarget.transform.position = ally.transform.position;
        gameObjectTarget.transform.rotation = ally.transform.rotation;

        LayerMask layerMask = LayerMask.NameToLayer("Enemy");
        layerMask |= ally.gameObject.layer;
        layerMask = ~layerMask;

        RaycastHit2D raycastHit2D = Physics2D.Raycast(transform.position, (Vector2)ally.transform.position - (Vector2)transform.position, Mathf.Infinity, layerMask);
        if( raycastHit2D.collider == circleCollider2D)
        {
            canSeeTarget = true;
        }


        circleCollider2D.enabled = false;
        gameObjectTarget.SetActive(false);

        return canSeeTarget;
    }*/

    AllyLogic GetPartyLeader()
    {
        AllyLogic partylead = null;
        foreach(AllyLogic allyLogic in AllyLogics)
        {
            if (allyLogic.partyLeader == true)
                partyLeader = allyLogic;
        }
        return partylead;
    }

    /**void superFriendshipBeam()
    {
        //Object.Instantiate(("assets/SFB"), transform.position, transform.rotation);
    }**/

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

            //GameObject childInstance = Instantiate(Square, shoot, Quaternion.FromToRotation(transform.position, shoot));


            rangedCoolDownInSeconds = rangedCoolDownInSecondsDefault;

            //GameObject Target = Instantiate(Square, mouseposition + (Vector2)transform.position, transform.rotation);
            //rangedCoolDownInSeconds = rangedCoolDownInSecondsDefault;
                
        }
        
    }

    public void TakeDamage(int amount)
    {
        playerHealth -= amount;
        if (playerHealth <= 0 && partyLeader == true)
            Debug.Log("Game Over");
    }
}
