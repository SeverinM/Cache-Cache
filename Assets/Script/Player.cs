using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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

    //Utilisé dans le drag and drop
    public GameObject holdGameObject;
    public Vector3 lastLegitPos;

    Button btnRight;
    Button btnLeft;

    [SyncVar(hook = nameof(ChangeRotate))]
    public bool CanRotate = false;

    [SerializeField]
    GameObject prefabUI;

    public void ChangeRotate(bool newValue)
    {
        if (btnLeft && btnRight)
        {
            btnRight.interactable = newValue;
            btnLeft.interactable = newValue;
        }
    }

    [Command]
    public void CmdChangeAuthority(GameObject gob, GameObject oldPlayer, GameObject newPlayer)
    {
        gob.GetComponent<NetworkIdentity>().RemoveClientAuthority(oldPlayer.GetComponent<NetworkIdentity>().connectionToClient);
        gob.GetComponent<NetworkIdentity>().AssignClientAuthority(newPlayer.GetComponent<NetworkIdentity>().connectionToClient);
        gob.GetComponent<Interactable>().Master = newPlayer;
    }

    public override void OnStartLocalPlayer()
    {
        GameObject gob = Instantiate(prefabUI);
        btnRight = gob.transform.GetChild(0).GetComponent<Button>();
        btnLeft = gob.transform.GetChild(1).GetComponent<Button>();

        btnLeft.GetComponent<ButtonInteraction>().OnButtonInteracted += (x) =>
        {
            if (x)
            {
                CmdTryRotate(gameObject, -1);
            }
            else
            {
                CmdRelease(gameObject);
            }
        };

        btnRight.GetComponent<ButtonInteraction>().OnButtonInteracted += (x) =>
        {
            if (x)
            {
                CmdTryRotate(gameObject, 1);
            }
            else
            {
                CmdRelease(gameObject);
            }
        };
    }

    private void Update()
    {
        if (hasAuthority)
        {
            if (maquette == null) return;

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
    public void CmdInit(GameObject maq, GameObject manager, GameObject other)
    {
        man = manager;
        maquette = maq;
        otherPlayer = other;
        CanRotate = true;
    }

    [Command]
    public void CmdTryRotate(GameObject concerned, float value)
    {
        if (CanRotate)
        {
            concerned.GetComponent<Player>().OtherPlayer.GetComponent<Player>().CanRotate = false;
            RpcRotateAll(Vector3.up, value);
            OtherPlayer.GetComponent<Player>().RpcRotateAll(Vector3.up, value);
        }
    }

    [Command]
    public void CmdRelease(GameObject concerned)
    {
        concerned.GetComponent<Player>().OtherPlayer.GetComponent<Player>().CanRotate = true;
    }

    [ClientRpc]
    public void RpcName(string nm)
    {
        name = nm;
    }

    public void RelayInteraction(Interactable.TypeAction acts , Interactable inter , Vector3 position)
    {
        CmdInter(acts, inter.gameObject, position);
    }

    [Command]
    public void CmdInter(Interactable.TypeAction acts, GameObject inter, Vector3 position)
    {
        inter.GetComponent<Interactable>().Interaction(acts, position, false);
    }

    #endregion
}
