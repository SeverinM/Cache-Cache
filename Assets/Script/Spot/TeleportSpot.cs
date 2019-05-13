using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportSpot : Spot
{
    static TeleportSpot spot1;
    static TeleportSpot spot2;

    [SerializeField]
    float maxDistance = 500;

    [SerializeField]
    MoonPart PartieHaute;
    [SerializeField]
    MoonPart PartieBasse;

    public override void EnterSpot(Draggable dragg)
    {
    }

    public override void ExitSpot(Draggable dragg)
    {
    }

    public override void ReleaseSpot(Draggable dragg)
    {
        SetValue(dragg, false);
        dragg.transform.position = (this != spot1 ? spot1 : spot2).transform.position;
    }

    public override void SetValue(Draggable dragg, bool value)
    {
        if (Vector3.Distance(dragg.transform.position, transform.position) < maxDistance)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = value;
            gameObject.GetComponent<Collider>().enabled = value;
        }
            
    }

    private void Awake()
    {
        bool chosen = false;
        if (!spot1)
        {
            spot1 = this;
            chosen = true;
        }

        if (!spot2 && !chosen)
        {
            spot2 = this;
        }
    }
}
