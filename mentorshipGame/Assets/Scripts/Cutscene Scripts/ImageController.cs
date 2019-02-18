using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ImageController : MonoBehaviour 
{
    public float fadeTime = 1f;
    public GameObject nextImage = null;
    public GameObject prevImage = null;
    private GameObject oldImage = null;

    Color color = new Color();

    float opacityValue = 0.0f;
    bool fadeFlag = false;
    SpriteRenderer BGrenderer;

    private void Awake()
    {
        BGrenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        StartCoroutine(StartFade());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("RightArrow") && opacityValue == 1f)
        {
            NextImage();
        }

        if (Input.GetKeyDown("LeftArrow") && opacityValue == 1f)
        {
            PrevImage();
        }
    }

    protected virtual void NextImage()
    {
        Vector3 spwanposition = transform.position;
        spwanposition.z = spwanposition.z - 0.01f;
        if (nextImage == null)
            Debug.LogError("ya fuked up");
        GameObject newObject = Instantiate(nextImage, spwanposition, Quaternion.Euler(Vector3.zero));
        newObject.transform.parent = transform.parent;
        newObject.GetComponent<ImageController>().oldImage = gameObject;
    }

    void PrevImage()
    {
        Vector3 spwanposition = transform.position;
        spwanposition.z = spwanposition.z - 0.01f;
        if (prevImage == null)
            Debug.LogError("ya fuked up");
        GameObject newObject = Instantiate(prevImage, spwanposition, Quaternion.Euler(Vector3.zero));
        newObject.GetComponent<ImageController>().oldImage = gameObject;
    }

    IEnumerator StartFade()
    {
        color = BGrenderer.color;
        while (opacityValue < 1)
        {
            opacityValue += Time.deltaTime / fadeTime;
            opacityValue = Mathf.Min(1, opacityValue);

            color.a = opacityValue;
            BGrenderer.color = color;
            yield return null;
        }
        if (oldImage != null)
            Destroy(oldImage);
    }
}
