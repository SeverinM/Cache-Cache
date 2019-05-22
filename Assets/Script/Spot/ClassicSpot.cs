using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassicSpot : Spot
{
    [SerializeField]
    float maxDistance = 500;

    public override void PressSpot(Draggable dragg)
    {
        currentHold = null;
    }

    public override void ReleaseSpot(Draggable dragg)
    {
        dragg.transform.position = transform.position;
        SetValue(dragg, false);
        currentHold = dragg;
        dragg.CurrentSpot = this;
        dragg.transform.SetParent(transform);
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
    }

    public override void ResetSpot(Draggable dragg)
    {
        dragg.transform.position = transform.position;
        currentHold = dragg;
    }

    public override void EnterSpot(Draggable dragg)
    {
        base.EnterSpot(dragg);
        dragg.transform.position = transform.position;
    }

    public override void SetValue(Draggable dragg, bool value)
    {
        if (Vector3.Distance(dragg.transform.position , transform.position) < maxDistance && currentHold == null)
        {
            GetComponent<Collider>().enabled = value;
            GetComponent<MeshRenderer>().enabled = value;
        }
    }
}
