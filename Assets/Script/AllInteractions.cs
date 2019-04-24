using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class AllInteractions
{

    public const string INTERACTION_TRIGGER = "Interaction";

    public enum Actions
    {
        START_DRAG = 0,
        END_DRAG,
        MOVE_DRAG,
        SEARCH,
        TELEPORT,
        INTERACTION
    }

    //Convertit l'enum en delegate
    public static Interactable.InteractionDelegate GetDelegate(AllInteractions.Actions acts)
    {
        Interactable.InteractionDelegate output = delegate { };

        switch (acts)
        {
            case Actions.START_DRAG:
                output += START_DRAG;
                break;

            case Actions.END_DRAG:
                output += END_DRAG;
                break;

            case Actions.MOVE_DRAG:
                output += MOVE_DRAG;
                break;

            case Actions.TELEPORT:
                output += TELEPORT;
                break;

            case Actions.INTERACTION:
                output += INTERACT;
                break;
        }

        return output;
    }

    public static void START_DRAG(GameObject gob, GameObject master, Vector3 position)
    {
        Player plr = master.GetComponent<Player>();
        Interactable inter = gob.GetComponent<Interactable>();
        if (plr.holdGameObject == null && !inter.Dragg)
        {
            gob.GetComponent<Interactable>().Dragg = true;          
            plr.holdGameObject = inter.gameObject;
            gob.transform.parent = master.transform;
            plr.lastLegitPos = position;
            inter.SetAllSpots(true);
        }
    }

    public static void MOVE_DRAG(GameObject gob, GameObject master, Vector3 position)
    {
        bool found = false;
        Interactable inter = gob.GetComponent<Interactable>();
        foreach(RaycastHit hit in Physics.RaycastAll(master.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition)))
        {
            int index = inter.AllSpots.FindIndex(x => x.obj == hit.collider.gameObject);
            if (index >= 0)
            {
                found = true;
                gob.transform.position = hit.collider.gameObject.transform.position;
                break;
            }
        }

        if (!found)
            gob.transform.position = position + new Vector3(0, 10, 0);
        master.GetComponent<Player>().lastLegitPos = position + new Vector3(0, 5, 0);
    }

    public static void END_DRAG(GameObject gob, GameObject master, Vector3 position)
    {
        Interactable inter = gob.GetComponent<Interactable>();

        inter.Dragg = false;
        inter.SetAllSpots(false);

        Player pl = master.GetComponent<Player>();
        master.GetComponent<Player>().holdGameObject = null;
        gob.transform.parent = master.GetComponent<Player>().maquette.transform;
    }

    public static void TELEPORT(GameObject gob , GameObject master , Vector3 position)
    {
        Player plr = master.GetComponent<Player>();
        Player other = plr.OtherPlayer.GetComponent<Player>();
        Vector3 delta = (other.maquette.transform.position - plr.maquette.transform.position);
        Debug.Log(delta);
        plr.CmdMove(gob, gob.transform.position + (other.maquette.transform.position - plr.maquette.transform.position));
        plr.CmdChangeAuthority(gob, plr.gameObject, plr.OtherPlayer);
    }

    public static void INTERACT(GameObject gob, GameObject master, Vector3 position)
    {
        gob.GetComponent<Animator>().SetTrigger(INTERACTION_TRIGGER);
    }

    public static GameObject DuplicateVisual(GameObject gob)
    {
        GameObject output = new GameObject("copy");
        CopyComponent<MeshRenderer>(gob.GetComponent<MeshRenderer>(), output);
        CopyComponent<MeshFilter>(gob.GetComponent<MeshFilter>(), output);
        CopyComponent<BoxCollider>(gob.GetComponent<BoxCollider>(), output);
        output.transform.position = gob.transform.position;
        output.transform.parent = gob.transform.parent;
        output.transform.localScale = gob.transform.localScale;
        output.transform.parent = null;
        return output;
    }

    public static T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        var dst = destination.GetComponent(type) as T;
        if (!dst) dst = destination.AddComponent(type) as T;
        var fields = type.GetFields();
        foreach (var field in fields)
        {
            if (field.IsStatic) continue;
            field.SetValue(dst, field.GetValue(original));
        }
        var props = type.GetProperties();
        foreach (var prop in props)
        {
            if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
            prop.SetValue(dst, prop.GetValue(original, null), null);
        }
        return dst as T;
    }
}
