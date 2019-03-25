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
            if (maquette == null || maquette2 == null) return;
            if (Input.GetKey(KeyCode.A))
            {
                CmdAcquire(playerIdentity);
            }

            if (Input.GetKeyUp(KeyCode.A))
            {
                CmdRelease();
            }
        }
    }

    [Command]
    public void CmdAcquire(int identity)
    {
        if (man.GetComponent<ManagerPlayers>().HasLock(playerIdentity))
        {
            man.GetComponent<ManagerPlayers>().AcquireLock(playerIdentity);
            float value = (playerIdentity == 1 ? -1 : 1);
            RpcRotate(maquette, value);
            RpcRotate(maquette2, value);
        }
    }

    [Command]
    public void CmdRelease()
    {
        man.GetComponent<ManagerPlayers>().ReleaseLock();
    }

    [ClientRpc]
    void RpcRotate(GameObject obj , float value)
    {
        if (obj.GetComponent<NetworkIdentity>().hasAuthority)
            obj.transform.Rotate(Vector3.up, value);
    }
}
