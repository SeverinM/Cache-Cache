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

    float deltaY;
    public float DeltaY => deltaY;

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
        if (!asEcho)
        {
            deltaY = Input.mousePosition.y - previousMousePosition.y;
        }

        previousMousePosition = Input.mousePosition;
        float ratio = Mathf.Clamp(Vector3.Distance(originPosition, GetPart().transform.position) / spr.MaxDistance, 0, 1);
        Vector3 temporaryPosition = GetPart().transform.position + (transform.up * deltaY * (1 - ratio) * spr.forceAddition);

        //Evite qu'une partie passe au desssus de l'autre
        if (actualPart == PartMoon.LOW_PART && GetPart().transform.position.y > originPosition.y)
        {
            GetPart().transform.position = originPosition;
        }

        if (actualPart == PartMoon.HIGH_PART && GetPart().transform.position.y < originPosition.y)
        {
            GetPart().transform.position = originPosition;
        }
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
