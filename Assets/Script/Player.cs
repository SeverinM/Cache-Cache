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
    public void CmdInit(int identity, GameObject gob, GameObject gob2)
    {
        playerIdentity = identity;
        maquette = gob;
        maquette2 = gob2;
    }

    private void Update()
    {
        if (!hasAuthority) return;
        if (Input.GetKey(KeyCode.A))
        {
            if (maquette == null || maquette == null) return;

            ManagerPlayers man = GameObject.FindObjectOfType<ManagerPlayers>();
            if (man.HasLock(playerIdentity))
            {
                Debug.Log("lock accaparé");
                man.CmdAcquireLock(playerIdentity);
                float value = (playerIdentity == 1 ? -1 : 1);
                CmdRotate(maquette, value);
                CmdRotate(maquette2, value);
            }
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            Debug.Log("press A");
            GameObject.FindObjectOfType<ManagerPlayers>().CmdReleaseLock();
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
