using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AllInteractions
{
    public static void ENTER_INTERACTION(GameObject gob, Vector3 pos)
    {
    }

    public static void EXIT_INTERACTION(GameObject gob , Vector3 position)
    {
    }

    public static void START_INTERACTION(GameObject gob, float timeStamp)
    {
        gob.GetComponent<Interactable>().ToggleGrab();
    }

    public static void MOVE_INTERACTION(GameObject gob, Vector3 newPosition)
    {
        gob.GetComponent<Interactable>().RpcTeleport(newPosition);
    }

    public static void END_INTERACTION(GameObject gob)
    {
        gob.GetComponent<Interactable>().ToggleGrab();
    }

    public static Vector3 GetNextPosition(Transform trsf,Camera cam)
    {
        Debug.Log(cam.gameObject);
        Plane pl = new Plane(cam.transform.forward, trsf.position);
        float dist = Vector3.Distance(trsf.transform.position, cam.transform.position);
        Vector3 pos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
        Vector3 delta = (pos - cam.transform.position).normalized;
        return pl.ClosestPointOnPlane(cam.transform.position + (delta * dist));
    }
}
