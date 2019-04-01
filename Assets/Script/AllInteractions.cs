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
        maquette mqt = master.GetComponent<Player>().maquette.GetComponent<maquette>();
        if (mqt.holdGameObject == null)
        {
            Interactable inter = gob.GetComponent<Interactable>();
            mqt.holdGameObject = inter.gameObject;
            gob.transform.parent = master.transform;
            IEnumerator routine = inter.Move(0.25f, gob.transform.position, gob.transform.position + new Vector3(0,10,0));
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

        position.x *= master.GetComponent<Camera>().pixelWidth;
        position.y *= master.GetComponent<Camera>().pixelHeight;
        Ray ray = master.GetComponent<Camera>().ScreenPointToRay(position);
        foreach (RaycastHit hit in Physics.RaycastAll(ray,1000))
        {
            if (hit.collider.tag == "Maquette")
            {
                inter.StartCoroutine(inter.Move(0.25f, gob.transform.position, hit.point));
                master.GetComponent<Player>().maquette.GetComponent<maquette>().holdGameObject = null;
                gob.transform.parent = null;
                break;
            }
        }
    }
}
