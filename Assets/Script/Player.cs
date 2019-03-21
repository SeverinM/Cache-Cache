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
    public void RpcLook(Vector3 position)
    {
        if (hasAuthority)
        {
            transform.LookAt(position);
        }
    }
}
