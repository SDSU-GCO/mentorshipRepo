using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartGroup : MonoBehaviour
{
    [SerializeField]
    private List <HeartController> heartControllers;

    public void updateHeart(int newHP)
    {
        for (int i = 0; i < heartControllers.Count; i++)
        {
            if (i < newHP)
            {
                heartControllers[i].setFull();
            }
            else
            {
                heartControllers[i].setEmpty();
            }
        }
        return;
    }



}
