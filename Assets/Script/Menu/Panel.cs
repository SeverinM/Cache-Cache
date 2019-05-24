using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    [SerializeField]
    int index;
    public int Index => index;

    protected bool canInteract;
    public bool CanInteract
    {
        get
        {
            return canInteract;
        }
        set
        {
            canInteract = value;
            foreach(Interactable inter in gameObject.GetComponentsInChildren<Interactable>())
            {
                inter.CanInteract = value;
            }
        }
    }

    public virtual void SetValue(bool newValue)
    {
        gameObject.SetActive(newValue);
    }
}
