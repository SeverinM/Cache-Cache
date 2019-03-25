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
    Occupation playerIdentity;

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
    public void CmdInit(Occupation occ, GameObject gob, GameObject gob2)
    {
        playerIdentity = occ;
        maquette = gob;
        maquette2 = gob2;
        Debug.Log(GameObject.FindObjectOfType<ManagerPlayers>().HasLock(playerIdentity));
    }

    private void Update()
    {
        if (isLocalPlayer) return;
        if (Input.GetKey(KeyCode.A))
        {
            if (maquette == null || maquette == null) return;

            ManagerPlayers man = GameObject.FindObjectOfType<ManagerPlayers>();
            Debug.Log(playerIdentity);
            if (man.HasLock(playerIdentity))
            {
                man.CmdAcquireLock(playerIdentity);
                float value = (playerIdentity == Occupation.PLAYER_1 ? -1 : 1);
                CmdRotate(maquette, value);
                CmdRotate(maquette2, value);
            }
        }

        if (Input.GetKeyUp(KeyCode.A))
        {
            GameObject.FindObjectOfType<ManagerPlayers>().CmdReleaseLock();
        }
    }

    [Command]
    void CmdRotate(GameObject obj , float value)
    {
        Debug.Log(value);
        RpcRotate(obj, value);
    }

    [ClientRpc]
    void RpcRotate(GameObject obj , float value)
    {
        obj.transform.Rotate(Vector3.up, value);
    }
}
