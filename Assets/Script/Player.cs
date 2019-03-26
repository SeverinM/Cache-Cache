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
    public void RpcLook(Vector3 vec, int amount)
    {
        if (hasAuthority)
        {
            transform.position = vec + new Vector3(100, 100, 0);
            transform.RotateAround(vec, Vector3.up, amount);
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
            if (Input.GetKey(KeyCode.D))
            {
                CmdAcquire(playerIdentity, Vector3.up, playerIdentity == 1 ? -1 : 1, Space.World);
            }

            if (Input.GetKey(KeyCode.Q))
            {
                CmdAcquire(playerIdentity, Vector3.up, playerIdentity == 1 ? 1 : -1, Space.World);
            }

            if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.Z))
            {
                CmdRelease();
            }
        }
    }

    [Command]
    public void CmdAcquire(int identity, Vector3 axis, float value, Space spc)
    {
        if (man.GetComponent<ManagerPlayers>().HasLock(playerIdentity))
        {
            man.GetComponent<ManagerPlayers>().AcquireLock(playerIdentity);
            RpcRotate(maquette, value, axis, spc);
            RpcRotate(maquette2, value, axis, spc);
        }
    }

    [Command]
    public void CmdRelease()
    {
        man.GetComponent<ManagerPlayers>().ReleaseLock();
    }

    [ClientRpc]
    void RpcRotate(GameObject obj , float value, Vector3 axis, Space spc)
    {
        if (obj.GetComponent<NetworkIdentity>().hasAuthority)
        {
            Debug.Log(obj.transform.localEulerAngles.z + " / " + obj.transform.eulerAngles.z);
            float modifiedValue = obj.transform.eulerAngles.z;
            if (axis == Vector3.forward && (modifiedValue + value > 90 || modifiedValue + value < 0)) return;
            obj.transform.Rotate(axis, value, spc);
        }           
    }
}
