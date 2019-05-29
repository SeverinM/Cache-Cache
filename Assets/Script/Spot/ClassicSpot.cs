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
        SetValue(dragg, false);
        currentHold = dragg;
        dragg.CurrentSpot = this;
        dragg.transform.SetParent(transform);
        dragg.transform.localPosition = Vector3.zero;

        if (GetComponent<Collider>())
            GetComponent<Collider>().enabled = false;

        if (GetComponent<MeshRenderer>())
            GetComponent<MeshRenderer>().enabled = false;

        if (GetComponent<Light>())
            GetComponent<Light>().enabled = false;
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
        if (transform.parent && transform.parent.GetComponent<ToggleInteraction>() && !transform.parent.GetComponent<ToggleInteraction>().ToggleState) return;
        if (Vector3.Distance(dragg.transform.position , transform.position) < maxDistance && currentHold == null)
        {
            GetComponent<Collider>().enabled = value;

            if (GetComponent<MeshRenderer>())
                GetComponent<MeshRenderer>().enabled = value;

            if (GetComponent<ParticleSystem>())
            {
                if (value)
                    GetComponent<ParticleSystem>().Play();
                else
                    GetComponent<ParticleSystem>().Stop();
            }

            if (GetComponent<Light>())
            {
                GetComponent<Light>().enabled = value;
            }
        }
    }
}
