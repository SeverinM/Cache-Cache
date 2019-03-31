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
        if (master.GetComponent<Player>().HoldGameObject == null)
        {
            Interactable inter = gob.GetComponent<Interactable>();
            master.GetComponent<Player>().HoldGameObject = inter.gameObject;
            gob.transform.parent = master.transform;
            IEnumerator routine = inter.Move(0.25f, gob.transform.position, master.transform.position + (master.transform.forward * 100));
            inter.StartCoroutine(routine);
            inter.CurrentCoroutine = routine;
        }
    }

    public static void MOVE_INTERACTION(GameObject gob, GameObject master, Vector3 position)
    {
    }

    public static void END_INTERACTION(GameObject gob, GameObject master, Vector3 position)
    {
        Interactable inter = gob.GetComponent<Interactable>();
        if (inter.CurrentCoroutine != null)
        {
            inter.StopCoroutine(inter.CurrentCoroutine);
        }

        Ray ray = master.GetComponent<Camera>().ScreenPointToRay(position);
        foreach (RaycastHit hit in Physics.RaycastAll(ray,1000))
        {
            if (hit.collider.tag == "Maquette")
            {
                inter.StartCoroutine(inter.Move(0.25f, gob.transform.position, hit.point));
                master.GetComponent<Player>().HoldGameObject = null;
                gob.transform.parent = null;
                break;
            }
        }
    }

    public static Vector3 GetNextPosition(Transform trsf,Camera cam)
    {
        Plane pl = new Plane(cam.transform.forward, trsf.position);
        float dist = Vector3.Distance(trsf.transform.position, cam.transform.position);
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        float nb = 0;

        if (pl.Raycast(ray,out nb))
        {
            return ray.GetPoint(nb);
        }
        else
        {
            Debug.Log("rien");
            return Vector3.zero;
        }
    }
}
