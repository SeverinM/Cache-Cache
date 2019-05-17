using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spot : MonoBehaviour
{
    [SerializeField]
    protected Draggable currentHold = null;
    public Draggable CurrentHold
    {
        get
        {
            return currentHold;
        }
        set
        {            
            currentHold = value;
        }
    }

    public virtual void EnterSpot(Draggable dragg)
    {
        dragg.transform.position = transform.position;
    }

    public virtual void ExitSpot(Draggable dragg)
    {

    }

    public abstract void ReleaseSpot(Draggable dragg);
    public abstract void SetValue(Draggable dragg, bool value);
    public abstract void ResetSpot(Draggable dragg);
    public abstract void PressSpot(Draggable dragg);
    public virtual void HoldObjectLeft(Draggable dragg)
    {
        currentHold = null;
    }
}
