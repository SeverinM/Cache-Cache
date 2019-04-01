using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class maquette : NetworkBehaviour
{
    [SyncVar]
    public GameObject master;

    public GameObject holdGameObject;

    private void Update()
    {
        if (hasAuthority && Input.GetMouseButtonDown(0) && holdGameObject == null)
        {
            Ray ray = master.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            foreach(RaycastHit hit in Physics.RaycastAll(ray,1000))
            {
                if(hit.collider.GetComponent<Interactable>())
                {
                    holdGameObject = hit.collider.gameObject;
                    hit.collider.GetComponent<Interactable>().Interaction(Interactable.TypeAction.START_INTERACTION, master, Vector3.zero);
                    break;
                }
            }
        }
    }
}
