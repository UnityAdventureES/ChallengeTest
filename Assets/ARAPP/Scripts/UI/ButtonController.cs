using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent OnButtonPressed;
    public UnityEvent OnButtonRealeased;

    public float speedEffect = 1;
    public float scaleEffect = 1.2f;
    float speed;
    float scale = 1;
    public GameObject effect;

    void Start()
    {
        effect.SetActive(false);
        speed = speedEffect;
    }

    void Update()
    {
        if (effect.activeSelf)
        {
            scale += Time.deltaTime * speed;
            if (scale > scaleEffect)
            {
                speed = -speedEffect;
            }
            if (scale < scaleEffect - 0.1f)
            {
                speed = speedEffect;
            }
            effect.transform.localScale = new Vector3(scale, scale, 1);
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        effect.SetActive(true);
        OnButtonPressed?.Invoke();
        scale = 1;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        effect.SetActive(false);
        OnButtonRealeased?.Invoke();
    }

}
