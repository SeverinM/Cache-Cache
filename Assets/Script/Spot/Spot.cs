using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spot : MonoBehaviour
{
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

    public abstract void EnterSpot(Draggable dragg);
    public abstract void ExitSpot(Draggable dragg);
    public abstract void ReleaseSpot(Draggable dragg);
    public abstract void SetValue(Draggable dragg, bool value);
    public abstract void ResetSpot(Draggable dragg);
    public abstract void PressSpot(Draggable dragg);
}
