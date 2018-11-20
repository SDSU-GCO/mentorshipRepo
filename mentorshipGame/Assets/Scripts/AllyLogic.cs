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
    public int playerHealth;
    public int damage;
    public float moveSpeed;
    Vector2 velocity = new Vector2(0,0);

    [HideInInspector]
    public static List<AllyLogic> AllyLogics = new List<AllyLogic>();

    private void Awake()
    {
        weaponCoolDownInSeconds = weaponAttack.attackDelay;
        weaponCoolDownInSecondsDefault = weaponCoolDownInSeconds;
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

            Rigidbody2D my2DRigidbody = GetComponent<Rigidbody2D>();
            velocity.x = moveHorizontal * moveSpeed;
            velocity.y = moveVertical * moveSpeed;
            my2DRigidbody.velocity = velocity;
        }
        
        //getComponent.characterManager.Character.SpecialAttack;

        weaponCoolDownInSeconds = Mathf.Max(0, weaponCoolDownInSeconds - Time.deltaTime);
        if (Input.GetMouseButtonDown(0) && weaponCoolDownInSeconds == 0)
        {
            FireAttack();
            weaponCoolDownInSeconds = weaponCoolDownInSecondsDefault;
        }
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
    void CheckPartyLead()
    {
        if (this.partyLeader)
        {
            //this.NPCLogic.disable;
            //PartyLeaderController.enable;
        }

    }

    public void TakeDamage(int amount)
    {
        playerHealth -= amount;
        if (playerHealth <= 0 && partyLeader == true)
            Debug.Log("Game Over");
    }
}
