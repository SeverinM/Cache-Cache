﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spot : MonoBehaviour
{
    protected Draggable currentHold;
    public Draggable CurrentHold { get; set; }

    public abstract void EnterSpot(Draggable dragg);
    public abstract void ExitSpot(Draggable dragg);
    public abstract void ReleaseSpot(Draggable dragg);
    public abstract void SetValue(Draggable dragg, bool value);
}
