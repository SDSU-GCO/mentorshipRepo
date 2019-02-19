using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Image))]
public class HeartController : MonoBehaviour, IGameObjectAddedToHierarchy
{
    public HeartGroup heartGroup;
    private SpriteRenderer spriteRenderer;
    private Image image;
    bool IsInScene=false;

    [SerializeField]
    private Sprite fullHeart;
    [SerializeField]
    private Sprite emptyHeart;

    private void Awake()
    {
        if(spriteRenderer==null)
            spriteRenderer = GetComponent<SpriteRenderer>();
        if (image == null)
            image = GetComponent<Image>();
        image.sprite = spriteRenderer.sprite;
    }

    public void setFull()
    {
        image.sprite = spriteRenderer.sprite = fullHeart;
        return;
    }

    public void setEmpty()
    {
        image.sprite = spriteRenderer.sprite = emptyHeart;
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
        spriteRenderer = GetComponent<SpriteRenderer>();
        image = GetComponent<Image>();
        return;
    }
}

public interface IGameObjectAddedToHierarchy
{
    bool IsAdded();
    void AddToHierarchy();
}