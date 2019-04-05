using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class AllInteractions
{
    public enum Actions
    {
        START_DRAG = 0,
        END_DRAG = 1,
        MOVE_DRAG = 2,
        SEARCH = 3,
        TELEPORT = 4
    }

    public static Interactable.InteractionDelegate GetDelegate(int index)
    {
        Interactable.InteractionDelegate output = delegate { };
        Actions act = (Actions)index;

        switch (act)
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
            IEnumerator routine = inter.Move(0.25f, gob.transform.position, gob.transform.position + new Vector3(0,10,0));
            inter.StartCoroutine(routine);
            inter.CurrentCoroutine = routine;
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
        if (inter.CurrentCoroutine != null)
        {
            inter.StopCoroutine(inter.CurrentCoroutine);
        }
        inter.Dragg = false;
        inter.SetAllSpots(false);

        Player pl = master.GetComponent<Player>();
        inter.StartCoroutine(inter.Move(0.1f, gob.transform.position, pl.lastLegitPos));
        master.GetComponent<Player>().holdGameObject = null;
        gob.transform.parent = master.GetComponent<Player>().maquette.transform;
    }

    public static void TELEPORT(GameObject gob , GameObject master , Vector3 position)
    {
        Player plr = master.GetComponent<Player>();
        plr.CmdTeleport(gob, gob.transform.position + (plr.OtherPlayer.GetComponent<Player>().maquette.transform.position - plr.maquette.transform.position));
        plr.CmdChangeAuthority(gob, plr.gameObject, plr.OtherPlayer);
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
