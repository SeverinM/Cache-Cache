﻿using System.Collections;
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
    public GameObject copiedGob;
    public Vector3 lastLegitPos;

    [SyncVar]
    public int playerIdentity;

    [Command]
    public void CmdTest(GameObject gob)
    {
        Debug.Log(gob);
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

            if (Input.GetMouseButtonDown(0) && holdGameObject == null)
            {
                Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                foreach (RaycastHit hit in Physics.RaycastAll(ray, 1000))
                {
                    if (hit.collider.GetComponent<Interactable>())
                    {
                        hit.collider.GetComponent<Interactable>().Interaction(Interactable.TypeAction.START_INTERACTION, gameObject, Vector3.zero);
                        break;
                    }
                }
            }

            if (holdGameObject != null)
            {
                if (Input.GetMouseButtonUp(0))
                {
                    Vector3 mouse = Input.mousePosition;
                    mouse.x /= GetComponent<Camera>().pixelWidth;
                    mouse.y /= GetComponent<Camera>().pixelHeight;
                    holdGameObject.GetComponent<Interactable>().Interaction(Interactable.TypeAction.END_INTERACTION, gameObject, mouse);
                }
                else
                {
                    Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                    foreach (RaycastHit hit in Physics.RaycastAll(ray, 1000))
                    {
                        if (!hit.collider.GetComponent<Interactable>())
                        {
                            holdGameObject.GetComponent<Interactable>().Interaction(Interactable.TypeAction.MOVE_INTERACTION, gameObject, hit.point);
                            break;
                        }
                    }
                }
            }

            else
            {
                if (Input.GetMouseButtonDown(1))
                {
                    GameObject obj;
                    Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
                    foreach (RaycastHit hit in Physics.RaycastAll(ray, 1000))
                    {
                        if (hit.collider.GetComponent<Interactable>())
                        {
                            obj = hit.collider.gameObject;
                            Player plr = GetComponent<Player>();
                            obj.transform.position += new Vector3(0, 20, 0);
                            break;
                        }
                    }
                }

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
        maquette.transform.parent = null;
    }
    #endregion
}
