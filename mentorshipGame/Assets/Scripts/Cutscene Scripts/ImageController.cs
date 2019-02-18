using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ImageController : MonoBehaviour 
{
    public float fadeTime = 1f;
    bool stillNew = true;
    public GameObject prevImage = null;
    public GameObject nextImage = null;
    private GameObject oldImage = null;

    Color color = new Color();

    float opacityValue = 0.0f;
    bool fadeFlag = false;
    Image BGrenderer;

    private void Awake()
    {
        BGrenderer = GetComponent<Image>();
    }

    private void OnEnable()
    {
        StartCoroutine(StartFade());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow) && opacityValue == 1f && stillNew)
        {
            NextImage();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) && opacityValue == 1f && stillNew)
        {
            PrevImage();
        }
    }

    protected virtual void NextImage()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 spwanposition = Vector3.zero;
        Debug.Log(spwanposition);
        if (nextImage == null)
            Debug.LogError("ya fuked up");
        else
        {
            stillNew = false;
            GameObject newObject = Instantiate(nextImage, spwanposition, Quaternion.Euler(Vector3.zero));
            RectTransform newTransform = newObject.GetComponent<RectTransform>();
            newTransform.SetParent(rectTransform.parent);
            newTransform.localPosition = new Vector3(rectTransform.localPosition.x, rectTransform.localPosition.y, rectTransform.localPosition.z - 0.01f); ;
            newTransform.localScale = Vector3.one;
            newTransform.sizeDelta = new Vector2(0, 0);
            newObject.GetComponent<ImageController>().oldImage = gameObject;
        }
    }

    void PrevImage()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Vector3 spwanposition = rectTransform.position;
        spwanposition.z = spwanposition.z - 0.01f;
        if (prevImage == null)
            Debug.LogError("ya fuked up");
        else
        {
            stillNew = false;
            GameObject newObject = Instantiate(prevImage, spwanposition, Quaternion.Euler(Vector3.zero));
            RectTransform newTransform = newObject.GetComponent<RectTransform>();
            newTransform.SetParent(rectTransform.parent);
            newTransform.localPosition = (spwanposition);
            newTransform.localScale = Vector3.one;
            newTransform.sizeDelta = new Vector2(0, 0);
            newObject.GetComponent<ImageController>().oldImage = gameObject;
        }
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
