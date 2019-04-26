using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MoonPrefab : Interactable
{
    class Spring
    {
        public float MaxDistance { get; internal set; }
        public float forceAddition { get; internal set; }

        public Spring(float maxDist , float force)
        {
            MaxDistance = maxDist;
            forceAddition = force;
        }
    }

    enum PartMoon
    {
        LOW_PART,
        HIGH_PART
    }
    PartMoon actualPart;

    //Les deux parties de la lune
    public GameObject LowerPart;
    public GameObject HigherPart;

    Spring spr; 

    Vector3 previousMousePosition;
    Vector3 originPosition;

    void Start()
    {
        spr = new Spring(20, 2);
        originPosition = transform.position;

        if (Master.GetComponent<NetworkIdentity>().isServer)
        {
            actualPart = PartMoon.LOW_PART;
        }
        else
        {
            actualPart = PartMoon.HIGH_PART;
        }
    }

    public override void StartInteraction(bool asEcho = false)
    {
        previousMousePosition = Input.mousePosition;
    }

    public override void MoveInteraction(bool asEcho = false)
    {
        float deltaY = Input.mousePosition.y - previousMousePosition.y;
        previousMousePosition = Input.mousePosition;
        float distance = Vector3.Distance(originPosition, GetPart().transform.position);
        float ratio = distance / spr.MaxDistance;

        GetPart().
    }

    GameObject GetPart()
    {
        return (actualPart == PartMoon.LOW_PART ? LowerPart : HigherPart);
    }

    public override void EndInteraction(bool asEcho = false)
    {
        GetPart().transform.position = Vector3.zero;
    }
}
