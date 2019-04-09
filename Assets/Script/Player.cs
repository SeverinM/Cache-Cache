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

    public GameObject holdGameObject;
    

    public Vector3 lastLegitPos;

    [SyncVar]
    public int playerIdentity;

    [Command]
    public void CmdChangeAuthority(GameObject gob, GameObject oldPlayer, GameObject newPlayer)
    {
        gob.GetComponent<NetworkIdentity>().RemoveClientAuthority(oldPlayer.GetComponent<NetworkIdentity>().connectionToClient);
        gob.GetComponent<NetworkIdentity>().AssignClientAuthority(newPlayer.GetComponent<NetworkIdentity>().connectionToClient);
        gob.GetComponent<Interactable>().Master = newPlayer;
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

            if (Input.GetMouseButtonDown(0))
            {
                foreach(RaycastHit hit in Physics.RaycastAll(GetComponent<Camera>().ScreenPointToRay(Input.mousePosition)))
                {
                    if (hit.collider.GetComponent<Interactable>())
                    {
                        hit.collider.GetComponent<Interactable>().Interaction(Interactable.TypeAction.START_INTERACTION, hit.point);
                        break;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0) && holdGameObject)
            {
                holdGameObject.GetComponent<Interactable>().Interaction(Interactable.TypeAction.END_INTERACTION ,Vector3.zero);
            }
        }
    }

    #region RPC et command
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

    [Command]
    public void CmdTeleport(GameObject gob , Vector3 vec)
    {
        RpcTeleport(gob, vec);
    }

    [ClientRpc]
    public void RpcTeleport(GameObject gob , Vector3 vec)
    {
        gob.transform.position = vec;
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

    [ClientRpc]
    public void RpcName(string nm)
    {
        name = nm;
    }

    public void RelayInteraction(Interactable.TypeAction acts , Interactable inter , Vector3 position)
    {
        inter.CmdInteractionEcho(acts, inter.gameObject, position);
    }

    #endregion
}
