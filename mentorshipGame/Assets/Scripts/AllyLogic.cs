using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cs;
//using WeaponSupervisor;

public class AllyLogic : MonoBehaviour
{
    //bool PlayerAttack = false;
    //bool SpAvailable = false;
    public WeaponAttackController weaponAttack;
    public bool partyLeader;
    float weaponCoolDownInSeconds;
    float weaponCoolDownInSecondsDefault;
    public int playerHealth;
    public int damage;
    public float moveSpeed;
    public float enemySize=0.5f;
    Vector2 velocity = new Vector2(0,0);
    public float followDistanceMax;
    public float followDistanceMin;
    Rigidbody2D my2DRigidbody;
    List<AllyLogic> nearbyAllies = new List<AllyLogic>();
    Vector2 forcesFromAllies = new Vector2(0, 0);
    [SerializeField]
    float forceOfNearbyAllies = 1;
    AStar2DPathfindingAgentController aStar2DPathfindingAgentController;

    [HideInInspector]
    public static List<AllyLogic> AllyLogics = new List<AllyLogic>();
    

    private void Awake()
    {
        weaponCoolDownInSeconds = weaponAttack.attackDelay;
        weaponCoolDownInSecondsDefault = weaponCoolDownInSeconds;
        my2DRigidbody = GetComponent<Rigidbody2D>();
        aStar2DPathfindingAgentController = GetComponent<AStar2DPathfindingAgentController>();
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
            if (Input.GetMouseButtonDown(0) && weaponCoolDownInSeconds == 0)
            {
                FireAttack();
                weaponCoolDownInSeconds = weaponCoolDownInSecondsDefault;
            }
        }
        else
        {
            AllyLogic partyLeader = GetPartyLeader();
            if (partyLeader != null)
            {

                float distance = Vector2.Distance(partyLeader.transform.position, transform.position);
                if (canSeeTargetAlly(partyLeader, enemySize))
                {
                    Debug.Log("sees it");
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
                else if(aStar2DPathfindingAgentController.pointIsInGrid(transform.position) && aStar2DPathfindingAgentController.pointIsInGrid(partyLeader.transform.position))
                {
                    Debug.Log("blind");
                    //path to ally
                    aStar2DPathfindingAgentController.target = partyLeader.transform;
                    Vector2? nextPoint = aStar2DPathfindingAgentController.getNextPoint();
                    if(nextPoint != null)
                    {
                        Vector2 nextPosition = (Vector2)nextPoint;
                        my2DRigidbody.velocity = (nextPosition - (Vector2)transform.position).normalized * moveSpeed;
                    }
                }
            }
        }

        getNearbyAllies();
        foreach(AllyLogic ally in AllyLogics)
        {
            Vector2 tempForce = new Vector2(forceOfNearbyAllies, forceOfNearbyAllies);
            tempForce.Scale( ((Vector2)transform.position - (Vector2)ally.transform.position));
            forcesFromAllies += tempForce;
        }

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

    private void getNearbyAllies()
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
    }

    Vector2 offset = new Vector2(0, 0);
    GameObject gameObjectTarget;
    CircleCollider2D circleCollider2D;

    [SerializeField]
    GameObject collisionTargetPrefab = null;

    [SerializeField]
    float repulsionDistance;

    private bool canSeeTargetAlly(AllyLogic ally, float radius)
    {
        if(gameObjectTarget == null)
        {
            gameObjectTarget = Instantiate(collisionTargetPrefab);
        }


        bool canSeeTarget = false;
        gameObjectTarget.SetActive(true);
        
        CircleCollider2D temp = gameObjectTarget.GetComponent<CircleCollider2D>();

        Debug.Assert(temp == null, "Error: Target prefab MUST have a circle collider");

        circleCollider2D.enabled = true;
        circleCollider2D.radius = 0.05f;
        circleCollider2D.offset = offset;

        gameObjectTarget.transform.position = ally.transform.position;
        gameObjectTarget.transform.rotation = ally.transform.rotation;

        LayerMask layerMask = LayerMask.NameToLayer("Enemy");
        layerMask |= ally.gameObject.layer;
        layerMask = ~layerMask;
        RaycastHit2D raycastHit2D = Physics2D.CircleCast(transform.position, radius, (Vector2)ally.transform.position - (Vector2)transform.position, Mathf.Infinity, layerMask);
        if( raycastHit2D.collider == circleCollider2D)
        {
            canSeeTarget = true;
        }


        circleCollider2D.enabled = false;
        gameObjectTarget.SetActive(false);

        return canSeeTarget;
    }

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

    public void TakeDamage(int amount)
    {
        playerHealth -= amount;
        if (playerHealth <= 0 && partyLeader == true)
            Debug.Log("Game Over");
    }
}
