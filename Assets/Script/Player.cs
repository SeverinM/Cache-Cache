using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Player : NetworkBehaviour
{
    public Vector3 ToOtherPlayer { get; set; }

    //[SyncVar]
    GameObject maquette;

    //[SyncVar]
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
        if (isLocalPlayer)
        {
            transform.position = vec + new Vector3(100, 100, 0);
            transform.RotateAround(vec, Vector3.up, amount);
            transform.LookAt(vec);
        }
    }

    [Command]
    public void CmdInit(int identity, GameObject maq, GameObject manager)
    {
        playerIdentity = identity;
        man = manager;
        maquette = maq;
    }


    private void Update()
    {
        if (hasAuthority)
        {
            if (maquette == null) return;
            if (Input.GetKey(KeyCode.D))
            {
                if (man.GetComponent<ManagerPlayers>().HasLock(playerIdentity))
                {
                    CmdAcquire(maquette.transform.position, Vector3.up, playerIdentity == 1 ? 1 : -1);
                }
            }

            if (Input.GetKey(KeyCode.Q))
            {
                if (man.GetComponent<ManagerPlayers>().HasLock(playerIdentity))
                {
                    CmdAcquire(maquette.transform.position, Vector3.up, playerIdentity == 1 ? -1 : 1);
                }
            }

            if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.Z))
            {
                CmdRelease();
            }
        }
    }

    [Command]
    public void CmdAcquire(Vector3 pos, Vector3 axis, float value)
    {
        man.GetComponent<ManagerPlayers>().AcquireLock(playerIdentity);
        foreach(GameObject gob in GameObject.FindGameObjectsWithTag("Player"))
        {
            gob.GetComponent<Player>().RpcRotate(pos, value, axis);
        }
    }

    [Command]
    public void CmdRelease()
    {
        man.GetComponent<ManagerPlayers>().ReleaseLock();
    }

    [ClientRpc]
    void RpcRotate(Vector3 pos,float value, Vector3 axis)
    {
        if (hasAuthority)
        {
            Debug.Log(gameObject);
            transform.RotateAround(pos, axis, value);
        }           
    }

    [ClientRpc]
    void RpcRotateAll(Vector3 axis , float value)
    {
        foreach (GameObject gob in GameObject.FindGameObjectsWithTag("Swappable"))
        {
            if (gob.GetComponent<NetworkIdentity>().hasAuthority)
            {
                Debug.Log(maquette);
                gob.transform.RotateAround(maquette.transform.position, axis, value);
            }
        }
    }
}
