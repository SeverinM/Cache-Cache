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
    Vector3 resetPosition;

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
        resetPosition = GetPart().transform.position;
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
        else
        {
            deltaY = Echo.GetComponent<MoonPrefab>().DeltaY;
        }

        previousMousePosition = Input.mousePosition;
        float ratio = Mathf.Clamp(Vector3.Distance(originPosition, GetPart().transform.position) / spr.MaxDistance, 0, 1);

        //Evite de bloquer la lune aux extremites
        if (ratio == 1)
        {
            if (deltaY < 0 && actualPart == PartMoon.HIGH_PART) ratio -= 0.001f;
            if (deltaY > 0 && actualPart == PartMoon.LOW_PART) ratio += 0.001f;
        }
        Vector3 temporaryPosition = GetPart().transform.position + (transform.up * deltaY * (1 - ratio) * spr.forceAddition);


        if (actualPart == PartMoon.LOW_PART && GetPart().transform.position.y > resetPosition.y)
        {
            temporaryPosition = resetPosition;
        }

        if (actualPart == PartMoon.HIGH_PART && GetPart().transform.position.y < resetPosition.y)
        {
            temporaryPosition = resetPosition;
        }

        GetPart().transform.position = temporaryPosition;
    }

    GameObject GetPart()
    {
        return (actualPart == PartMoon.LOW_PART ? LowerPart : HigherPart);
    }

    public override void EndInteraction(bool asEcho = false)
    {
        GetPart().transform.position = resetPosition;
    }
}
