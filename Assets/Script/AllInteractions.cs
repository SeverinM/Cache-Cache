using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AllInteractions
{
    public static void ENTER_INTERACTION(GameObject gob, GameObject master, Vector3 position)
    {
    }

    public static void EXIT_INTERACTION(GameObject gob , GameObject master, Vector3 position)
    {
    }

    public static void START_INTERACTION(GameObject gob, GameObject master, Vector3 position)
    {
        Player plr = master.GetComponent<Player>();
        if (plr.holdGameObject == null)
        {
            Interactable inter = gob.GetComponent<Interactable>();
            plr.copiedGob = DuplicateVisual(gob);
            plr.holdGameObject = inter.gameObject;
            plr.copiedGob.GetComponent<MeshRenderer>().material = plr.holdGameObject.GetComponent<Interactable>().OtherMat;
            gob.transform.parent = master.transform;
            IEnumerator routine = inter.Move(0.25f, gob.transform.position, gob.transform.position + new Vector3(0,10,0));
            inter.StartCoroutine(routine);
            inter.CurrentCoroutine = routine;
            plr.lastLegitPos = position;
        }
    }

    public static void MOVE_INTERACTION(GameObject gob, GameObject master, Vector3 position)
    {
        if (!gob.GetComponent<Interactable>().dragg)
        {
            gob.transform.position = position + new Vector3(0, 10, 0);
            master.GetComponent<Player>().lastLegitPos = position + new Vector3(0, 5, 0);
        }
    }

    public static void END_INTERACTION(GameObject gob, GameObject master, Vector3 position)
    {
        Interactable inter = gob.GetComponent<Interactable>();
        if (inter.CurrentCoroutine != null)
        {
            inter.dragg = false;
            inter.StopCoroutine(inter.CurrentCoroutine);
        }

        Player pl = master.GetComponent<Player>();
        inter.StartCoroutine(inter.Move(0.1f, gob.transform.position, pl.lastLegitPos));
        master.GetComponent<Player>().holdGameObject = null;
        gob.transform.parent = master.GetComponent<Player>().maquette.transform;
        GameObject.Destroy(pl.copiedGob);
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
