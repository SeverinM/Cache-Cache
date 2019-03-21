using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Player : NetworkBehaviour
{
    public static int count = 0;
    public int id;

    void Awake()
    {
        id = count;
        count++;
    }


    [ClientRpc]
    public void RpcUpdateCam()
    {
        if (hasAuthority)
        {
            foreach (Camera cam in GameObject.FindObjectsOfType<Camera>())
            {
                if (cam.gameObject != gameObject)
                {
                    cam.enabled = false;
                }
                else
                {
                    cam.enabled = true;
                }
            }
        }
    }

    [ClientRpc]
    public void RpcSetMaquette(GameObject maquette)
    {
        if (hasAuthority)
        {
            Debug.Log(maquette);
            maquette.transform.position = transform.position + (Random.onUnitSphere * 3);
            transform.LookAt(maquette.transform.position);
            transform.Rotate(Vector3.right, 90);
        }
    }
}
