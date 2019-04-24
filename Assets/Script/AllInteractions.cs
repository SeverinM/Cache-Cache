using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class AllInteractions
{

    public const string INTERACTION_TRIGGER = "Interaction";

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
