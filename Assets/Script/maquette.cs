using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class maquette : NetworkBehaviour
{
    [SyncVar]
    public GameObject master;

    public GameObject holdGameObject;
    public GameObject copiedGob;

    public Vector3 lastLegitPos;

    private void Update()
    {
        if (!hasAuthority) return;
        if (Input.GetMouseButtonDown(0) && holdGameObject == null)
        {
            Ray ray = master.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            foreach(RaycastHit hit in Physics.RaycastAll(ray,1000))
            {
                if(hit.collider.GetComponent<Interactable>())
                {
                    hit.collider.GetComponent<Interactable>().Interaction(Interactable.TypeAction.START_INTERACTION, master, Vector3.zero);
                    break;
                }
            }
        }

        if (holdGameObject != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Vector3 mouse = Input.mousePosition;
                mouse.x /= master.GetComponent<Camera>().pixelWidth;
                mouse.y /= master.GetComponent<Camera>().pixelHeight;
                holdGameObject.GetComponent<Interactable>().Interaction(Interactable.TypeAction.END_INTERACTION, master, mouse);
            }
            else
            {
                Ray ray = master.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                foreach (RaycastHit hit in Physics.RaycastAll(ray, 1000))
                {
                    if (!hit.collider.GetComponent<Interactable>())
                    {
                        holdGameObject.GetComponent<Interactable>().Interaction(Interactable.TypeAction.MOVE_INTERACTION, master, hit.point);
                        break;
                    }
                }
            }
        }

        else
        {
            if (Input.GetMouseButtonDown(1))
            {
                Ray ray = master.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                foreach (RaycastHit hit in Physics.RaycastAll(ray, 1000))
                {
                    if (hit.collider.GetComponent<Interactable>())
                    {
                        Player plr = master.GetComponent<Player>();
                        hit.collider.transform.parent = null;
                        plr.Move(hit.collider.gameObject);
                        break;
                    }
                }
            }
        }
    }
}
