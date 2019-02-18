using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HeartController : MonoBehaviour, IGameObjectAddedToHierarchy
{
    public HeartGroup heartGroup;
    bool IsInScene=false;

    [SerializeField]
    private Sprite fullHeart;
    [SerializeField]
    private Sprite emptyHeart;


    public void setFull()
    {
        GetComponent<SpriteRenderer>().sprite = fullHeart;
        return;
    }

    public void setEmpty()
    {
        GetComponent<SpriteRenderer>().sprite = emptyHeart;
        return;
    }
   
    public bool IsAdded()
    {
        return IsInScene;
    }
    public void AddToHierarchy()
    {
        IsInScene = true;
        heartGroup = FindObjectOfType<HeartGroup>();
        return;
    }
}

public interface IGameObjectAddedToHierarchy
{
    bool IsAdded();
    void AddToHierarchy();
}