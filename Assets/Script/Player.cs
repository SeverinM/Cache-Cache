using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Player : NetworkBehaviour
{
    public Vector3 ToOtherPlayer { get; set; }

    [SyncVar]
    public GameObject maquette;

    [SyncVar]
    GameObject otherPlayer;
    public GameObject OtherPlayer => otherPlayer;

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
        if (isLocalPlayer)
        {
            transform.position = vec + new Vector3(100, 100, 0);
            transform.RotateAround(vec, Vector3.up, amount);
            transform.LookAt(vec);
        }
    }

    [Command]
    public void CmdInit(int identity, GameObject maq, GameObject manager, GameObject other)
    {
        playerIdentity = identity;
        man = manager;
        maquette = maq;
        otherPlayer = other;
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

            if (!Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.Q))
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
            gob.GetComponent<Player>().RpcRotateAll(axis, value);
        }
    }

    [Command]
    public void CmdRelease()
    {
        man.GetComponent<ManagerPlayers>().ReleaseLock();
    }

    [ClientRpc]
    void RpcRotateAll(Vector3 axis , float value)
    {
        if (hasAuthority)
        {
            transform.RotateAround(maquette.transform.position, axis, value);
        }
    }

    public void Move(GameObject who)
    {
        CmdSpawn(who, gameObject, who.transform.position + (OtherPlayer.GetComponent<Player>().maquette.transform.position - maquette.transform.position));
    }

    [Command]
    public void CmdSpawn(GameObject who, GameObject player, Vector3 position)
    {
        Debug.Log(who);
        who.transform.parent = null;
        GameObject instance = Instantiate(who);
        instance.transform.position = position;
        NetworkServer.SpawnWithClientAuthority(instance, player);
        NetworkServer.UnSpawn(who);
        Destroy(who);
        //NetworkServer.SpawnWithClientAuthority(who, player.GetComponent<NetworkIdentity>().connectionToClient);
    }
}
