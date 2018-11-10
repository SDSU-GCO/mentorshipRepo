using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using WeaponSupervisor;

public class AllyLogic : MonoBehaviour
{
	bool PlayerAttack = false;
    bool SpAvailable = false;
    public WeaponAttackController weaponAttack;


    float weaponCoolDownInSeconds;
    float weaponCoolDownInSecondsDefault;

    List<AllyLogic> AllyLogics = new List<AllyLogic>();

    private void Awake()
    {
        weaponCoolDownInSeconds = weaponAttack.attackDelay;
        weaponCoolDownInSecondsDefault = weaponCoolDownInSeconds;
    }

    //AllyLogics.Add

    void Update ()
    {
        //getComponent.characterManager.Character.SpecialAttack;

        weaponCoolDownInSeconds = Mathf.Max(0, weaponCoolDownInSeconds - Time.deltaTime);
        if ((Input.GetMouseButtonDown(0) && weaponCoolDownInSeconds == 0))
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
            
            Debug.Log("Rotation: " + rotation + "\n");
            GameObject child = Instantiate(weaponAttack.gameObject, spawnLocation, Quaternion.Euler(0, 0, rotation));
            child.transform.SetParent(transform);


        }
    }
}
