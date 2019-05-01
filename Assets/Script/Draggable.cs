using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Draggable : Interactable
{
    class SpotDragAndDropInter
    {
        public Vector3 position;
        public GameObject instance;
        public SpotDragAndDropInter(Vector3 firstPosition, GameObject firstInst)
        {
            position = firstPosition;
            instance = firstInst;
        }
    }

    Spot currentSpot;
    Vector3 startPosition;

    List<SpotDragAndDropInter> allSpots = new List<SpotDragAndDropInter>();
    List<SpotDragAndDropInter> AllSpots => allSpots;

    [HideInInspector]
    Material otherMat;

    public enum TypeAction
    {
        START_INTERACTION,
        END_INTERACTION,
        MOVE_INTERACTION,
        ENTER_INTERACTION,
        EXIT_INTERACTION
    }

    bool _canInteract = true;
    public bool CanInteract => _canInteract;

    public override void StartInteraction(bool asEcho)
    {
        Player plr = Master.GetComponent<Player>();
        //Cant hold 2 gameobject at the same time
        if (plr.holdGameObject == null)
        {
            Vector3 position = transform.position;
            startPosition = position;
            transform.parent = Master.transform;
            plr.lastLegitPos = position;

            //Enable all spots
            SetAllSpots(true);
        }
    }

    public override void MoveInteraction(bool asEcho)
    {
        bool found = false;
        Spot before = currentSpot;
        Vector3 position = transform.position;

        //Seach first valid collision
        foreach (RaycastHit hit in Physics.RaycastAll(Master.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition)))
        {
            position = hit.point;
            Spot sp = hit.collider.GetComponent<Spot>();
            if (hit.collider.tag == "Maquette") break;
            if (sp != null)
            {
                found = true;
                currentSpot = sp;
                break;
            }
        }

        if (!found)
        {
            currentSpot = null;
        }

        if (currentSpot != before)
        {
            if (before) before.CursorExit(gameObject);
            if (currentSpot) currentSpot.CursorEnter(gameObject);
        }

        Master.GetComponent<Player>().lastLegitPos = position;
        transform.position = position;
    }

    public override void EndInteraction(bool asEcho)
    {
        if (currentSpot)
        {
            //Laché sur le spot
            currentSpot.CursorRelease(gameObject);
            currentSpot = null;
        }
        else
        {
            transform.position = startPosition;
        }

        SetAllSpots(false);
        transform.parent = null;
    }

    [ClientRpc]
    public void RpcAddSpot(Vector3 position, GameObject gob)
    {
        allSpots.Add(new SpotDragAndDropInter(position, gob));
        gob.GetComponent<Spot>().SetState(false);
    }

    public void AddSpot(Vector3 position, GameObject gob)
    {
        allSpots.Add(new SpotDragAndDropInter(position, gob));
        gob.GetComponent<Spot>().SetState(false);
    }

    public void SetAllSpots(bool value)
    {
        foreach (SpotDragAndDropInter sp in allSpots)
        {
            sp.instance.GetComponent<Spot>().SetState(value);
        }
    }
}
