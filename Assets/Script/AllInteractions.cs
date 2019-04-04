﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

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

    public Interactable.InteractionDelegate GetDelegate(List<int> index)
    {
        Interactable.InteractionDelegate output = delegate { };

        foreach (int ind in index)
        {
            Actions act = (Actions)ind;

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
            }
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
            plr.CopiedGob = DuplicateVisual(gob);
            plr.holdGameObject = inter.gameObject;
            plr.CopiedGob.GetComponent<MeshRenderer>().material = plr.holdGameObject.GetComponent<Interactable>().OtherMat;
            gob.transform.parent = master.transform;
            IEnumerator routine = inter.Move(0.25f, gob.transform.position, gob.transform.position + new Vector3(0,10,0));
            inter.StartCoroutine(routine);
            inter.CurrentCoroutine = routine;
            plr.lastLegitPos = position;
        }
    }

    public static void MOVE_DRAG(GameObject gob, GameObject master, Vector3 position)
    {
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

        Player pl = master.GetComponent<Player>();
        inter.StartCoroutine(inter.Move(0.1f, gob.transform.position, pl.lastLegitPos));
        master.GetComponent<Player>().holdGameObject = null;
        gob.transform.parent = master.GetComponent<Player>().maquette.transform;
        GameObject.Destroy(pl.CopiedGob);
    }

    public static void TELEPORT(GameObject gob , GameObject master , Vector3 position)
    {
        Player plr = master.GetComponent<Player>();
        GameObject.Destroy(plr.CopiedGob);
        plr.CmdTeleport(gob, gob.transform.position + (plr.OtherPlayer.GetComponent<Player>().maquette.transform.position - plr.maquette.transform.position));
        plr.CmdChangeAuthority(gob, plr.gameObject, plr.OtherPlayer);
    }

    public static GameObject DuplicateVisual(GameObject gob)
    {
        GameObject output = new GameObject("copy");
        CopyComponent<MeshRenderer>(gob.GetComponent<MeshRenderer>(), output);
        CopyComponent<MeshFilter>(gob.GetComponent<MeshFilter>(), output);
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
