using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Player : NetworkBehaviour
{
    public Vector3 ToOtherPlayer { get; set; }

    [SyncVar]
    GameObject maquette;

    [SyncVar]
    GameObject maquette2;

    [SyncVar]
    GameObject man;

    [SyncVar]
    public int playerIdentity;

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
    public void RpcLook(Vector3 vec)
    {
        if (hasAuthority)
        {
            transform.position = vec + new Vector3(100, 100, 0);
            transform.LookAt(vec);
        }
    }

    [Command]
    public void CmdInit(int identity, GameObject gob, GameObject gob2, GameObject manager)
    {
        playerIdentity = identity;
        maquette = gob;
        maquette2 = gob2;
        man = manager;
    }

    private void Update()
    {
        if (hasAuthority)
        {
            if (Input.GetKey(KeyCode.A))
            {
                if (maquette == null || maquette == null) return;
                //if (man.HasLock(playerIdentity))
                //{
                //    man.CmdAcquireLock(playerIdentity);
                //    float value = (playerIdentity == 1 ? -1 : 1);
                //    CmdRotate(maquette, value);
                //    CmdRotate(maquette2, value);
                //}

            }

            if (Input.GetKeyUp(KeyCode.A))
            {
                man.GetComponent<ManagerPlayers>().CmdReleaseLock();
            }
        }
    }

    [Command]
    void CmdRotate(GameObject obj , float value)
    {
        RpcRotate(obj, value);
    }

    [ClientRpc]
    void RpcRotate(GameObject obj , float value)
    {
        obj.transform.Rotate(Vector3.up, value);
    }
}
