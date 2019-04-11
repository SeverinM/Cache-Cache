using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonInteraction : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler
{
    public delegate void BtnDelegate(bool pressed);
    public event BtnDelegate OnButtonInteracted;
    bool pressed = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
        if (OnButtonInteracted != null)
            OnButtonInteracted(false);
    }

    public void Update()
    {
        if (pressed && OnButtonInteracted != null)
        {
            OnButtonInteracted(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        pressed = false;
    }
}
