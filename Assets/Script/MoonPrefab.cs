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

    public enum PartMoon
    {
        LOW_PART,
        HIGH_PART, 
        NONE
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

    public float maxDistance = 20;
    bool canInteract = true;

    void Start()
    {
        spr = new Spring(maxDistance, 2);
        originPosition = transform.position;
        actualPart = PartMoon.NONE;
    }

    public override void StartInteraction(bool asEcho = false)
    {
        if (!canInteract) return;
        previousMousePosition = Input.mousePosition;

        foreach(RaycastHit hit in Physics.RaycastAll(Master.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition)))
        {
            if (hit.collider.gameObject == HigherPart && Echo.GetComponent<MoonPrefab>().actualPart != PartMoon.HIGH_PART)
            {
                actualPart = PartMoon.HIGH_PART;
                resetPosition = HigherPart.transform.position;
                break;
            }

            if (hit.collider.gameObject == LowerPart && Echo.GetComponent<MoonPrefab>().actualPart != PartMoon.LOW_PART)
            {
                actualPart = PartMoon.LOW_PART;
                resetPosition = LowerPart.transform.position;
                break;
            }
        }
    }

    public override void MoveInteraction(bool asEcho = false)
    {
        if (!canInteract) return;
        if (!asEcho)
        {
            deltaY = Input.mousePosition.y - previousMousePosition.y;

            previousMousePosition = Input.mousePosition;

            if (!GetPart()) return;
            float ratio = GetRatio(GetPart());

            //Evite de bloquer la lune aux extremites
            if (ratio == 1)
            {
                if (deltaY < 0 && actualPart == PartMoon.HIGH_PART) ratio -= 0.001f;
                if (deltaY > 0 && actualPart == PartMoon.LOW_PART) ratio -= 0.001f;
            }

            Vector3 temporaryPosition = GetPart().transform.position + (transform.up * deltaY * (1 - ratio) * spr.forceAddition);
            if (actualPart == PartMoon.LOW_PART && GetPart().transform.position.y > resetPosition.y && deltaY > 0)
            {
                temporaryPosition = resetPosition;
            }

            if (actualPart == PartMoon.HIGH_PART && GetPart().transform.position.y < resetPosition.y && deltaY < 0)
            {
                temporaryPosition = resetPosition;
            }

            GetPart().transform.position = temporaryPosition;
            CmdUpdatePosition(Master.GetComponent<Player>().OtherPlayer, actualPart, temporaryPosition);

            // Les deux parties one été tirés
            if (GetRatio(LowerPart) == 1 && GetRatio(HigherPart) == 1)
            {
                Lock();
            }
        }

        else
        {
            MoonPrefab other = Echo.GetComponent<MoonPrefab>();
            Vector3 toLow = other.LowerPart.transform.position - other.transform.position;
            Vector3 toHigh = other.HigherPart.transform.position - other.transform.position;
            LowerPart.transform.position = transform.position + toLow;
            HigherPart.transform.position = transform.position + toHigh;
        }
    }

    GameObject GetPart()
    {
        if (actualPart == PartMoon.NONE)
            return null;
        return  (actualPart == PartMoon.LOW_PART ? LowerPart : HigherPart);
    }

    public override void EndInteraction(bool asEcho = false)
    {
        if (!canInteract) return;
        if (!asEcho)
        {
            GetPart().transform.position = resetPosition;
            CmdUpdatePosition(Master.GetComponent<Player>().OtherPlayer, actualPart, resetPosition);

            actualPart = PartMoon.NONE;
            CmdUpdatePosition(Master.GetComponent<Player>().OtherPlayer, PartMoon.NONE, resetPosition);
        }
            
        else
        {
            MoonPrefab other = Echo.GetComponent<MoonPrefab>();
            Vector3 toLow = other.LowerPart.transform.position - other.transform.position;
            Vector3 toHigh = other.HigherPart.transform.position - other.transform.position;
            LowerPart.transform.position = transform.position + toLow;
            HigherPart.transform.position = transform.position + toHigh;
        }
    }

    [Command]
    public void CmdUpdatePosition(GameObject gob , PartMoon part , Vector3 position)
    {
        TargetUpdatePosition(gob.GetComponent<NetworkIdentity>().connectionToClient, part, position);
    }

    [TargetRpc]
    public void TargetUpdatePosition(NetworkConnection conn, PartMoon part , Vector3 position)
    {
        actualPart = part;
        if (actualPart == PartMoon.HIGH_PART)
        {
            HigherPart.transform.position = position;
        }

        if (actualPart == PartMoon.LOW_PART)
        {
            LowerPart.transform.position = position;
        }
    }

    float GetRatio(GameObject gob)
    {
        return Mathf.Clamp(Vector3.Distance(originPosition, gob.transform.position) / spr.MaxDistance, 0, 1);
    }

    void Lock()
    {
        Debug.Log("lock");
        canInteract = false;
    }
}
