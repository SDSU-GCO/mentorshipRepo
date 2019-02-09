using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(AllyLogic))]
public class HeartContainers : MonoBehaviour
{
    public List<GameObject> heartContainers;

    public static HeartContainers hearts = null;

    private static AllyLogic allyLogicRef;

    private void Awake()
    {
        hearts = this;
    }

    // Use this for initialization
    void Start()
    {
        allyLogicRef = AllyLogic.GetPartyLeaderGlobal();
        heartContainers.Add(gameObject);
        Debug.Log(heartContainers.Count);
    }
    /// <summary>
    /// gets a reference to playerhealth and updates the heart containers/game over accordingly
    /// </summary>
    public void UpdateHP()
    {
        Debug.Log(heartContainers.Count);
        for (int i = 0; i < heartContainers.Count; i++)
        {
            if (i <= allyLogicRef.GetPartyLeader().playerHealth)
            {
                heartContainers[i].GetComponent<Animator>().enabled = true;
                Debug.Log(true);
            }
            else
            {
                heartContainers[i].GetComponent<Animator>().enabled = false;
                Debug.Log(false);
            }
        }

    }
}
