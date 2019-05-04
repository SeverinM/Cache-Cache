using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassicSpot : Spot
{
    public override void EnterSpot(Draggable dragg)
    {
    }

    public override void ExitSpot(Draggable dragg)
    {
    }

    public override void ReleaseSpot(Draggable dragg)
    {
        dragg.transform.position = transform.position;
    }

    public override void SetValue(bool value)
    {
        gameObject.SetActive(value);
    }
}
