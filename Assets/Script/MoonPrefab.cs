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
    public GameObject PrefabSpot;

    float deltaY;
    public float DeltaY => deltaY;

    Spring spr; 

    Vector3 previousMousePosition;
    Vector3 originPosition;

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
        if (!canInteract)
        {
            Debug.Log("pas interaction");
        }

        if (!asEcho)
        {
            previousMousePosition = Input.mousePosition;
            foreach (RaycastHit hit in Physics.RaycastAll(Master.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition)))
            {
                if (hit.collider.gameObject == HigherPart)
                {
                    if (Echo.GetComponent<MoonPrefab>().actualPart != PartMoon.HIGH_PART)
                    {
                        actualPart = PartMoon.HIGH_PART;
                    }
                    break;
                }

                if (hit.collider.gameObject == LowerPart)
                {
                    if (Echo.GetComponent<MoonPrefab>().actualPart != PartMoon.LOW_PART)
                    {
                        actualPart = PartMoon.LOW_PART;
                    }
                    break;
                }
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

            Vector3 temporaryPosition = GetPart().transform.localPosition + (transform.up * deltaY * (1 - ratio) * 0.05f);

            //Low part always below
            if (actualPart == PartMoon.LOW_PART && temporaryPosition.y <= 0)
            {
                GetPart().transform.localPosition = temporaryPosition;
                //Move on remote
                CmdUpdatePosition(Master.GetComponent<Player>().OtherPlayer, actualPart, temporaryPosition);
            }
            
            if (actualPart == PartMoon.HIGH_PART && temporaryPosition.y >= 0)
            {
                GetPart().transform.localPosition = temporaryPosition;
                //Move on remote
                CmdUpdatePosition(Master.GetComponent<Player>().OtherPlayer, actualPart, temporaryPosition);
            }

            // Les deux parties one été tirés
            if (GetRatio(LowerPart) == 1 && GetRatio(HigherPart) == 1)
            {
                CmdLock();
            }
        }

        else
        {
            MoonPrefab other = Echo.GetComponent<MoonPrefab>();

            //Mimic only if not hold
            if (actualPart != PartMoon.LOW_PART)
                LowerPart.transform.localPosition = other.LowerPart.transform.localPosition;
            if (actualPart != PartMoon.HIGH_PART)
                HigherPart.transform.localPosition = other.HigherPart.transform.localPosition;
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
            Debug.Log("Fin interaction");
            GetPart().transform.localPosition = Vector3.zero;
            CmdUpdatePosition(Master.GetComponent<Player>().OtherPlayer, actualPart, Vector3.zero);

            actualPart = PartMoon.NONE;
            CmdUpdatePosition(Master.GetComponent<Player>().OtherPlayer, PartMoon.NONE, Vector3.zero);
        }
            
        else
        {
            MoonPrefab other = Echo.GetComponent<MoonPrefab>();

            //Mimic only if not hold
            if (actualPart != PartMoon.LOW_PART)
                LowerPart.transform.localPosition = other.LowerPart.transform.localPosition;
            if (actualPart != PartMoon.HIGH_PART)
                HigherPart.transform.localPosition = other.HigherPart.transform.localPosition;
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
            HigherPart.transform.localPosition = position;
        }

        if (actualPart == PartMoon.LOW_PART)
        {
            LowerPart.transform.localPosition = position;
        }
    }

    [Command]
    public void CmdLock()
    {
        RpcLock();
    }

    [ClientRpc]
    public void RpcLock()
    {
        Lock();
    }

    float GetRatio(GameObject gob)
    {
        return Mathf.Clamp(Vector3.Distance(originPosition, gob.transform.position) / spr.MaxDistance, 0, 1);
    }

    void Lock()
    {
        canInteract = false;
        GameObject instance = Instantiate(PrefabSpot, transform.position, Quaternion.identity);
        GetComponent<SphereCollider>().enabled = false;
        foreach(Draggable dragg in FindObjectsOfType<Draggable>())
        {
            dragg.AddSpot(instance.transform.position, instance);
        }
    }
}
