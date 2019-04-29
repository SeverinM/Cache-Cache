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

    public Vector3 deltaCam = new Vector3(100, 50, 0);

    [SyncVar]
    GameObject otherPlayer;
    public GameObject OtherPlayer => otherPlayer;

    [SyncVar]
    GameObject man;

    public AnimationCurve curve;

    //Utilisé dans le drag and drop
    [HideInInspector]
    public GameObject holdGameObject;

    [HideInInspector]
    public Vector3 lastLegitPos;

    Button btnRight;
    Button btnLeft;

    [SyncVar(hook = nameof(ChangeRotate))]
    public bool CanRotate = false;

    public GameObject prefabUI;

    float maxZoom;
    float minZoom = 30;
    float delta = 0;
    float speedZoom = 0;
    Vector3 ForwardMaquette => (maquette.transform.position - transform.position).normalized;
    Vector3 target;

    public enum TypeAction
    {
        START_INTERACTION,
        MOVE_INTERACTION,
        END_INTERACTION
    }

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
        maxZoom = GetComponent<Camera>().fieldOfView;

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

            //On proc la fin d'interaction pour l'objet tenu
            if (Input.GetMouseButtonDown(0))
            {
                foreach(RaycastHit hit in Physics.RaycastAll(GetComponent<Camera>().ScreenPointToRay(Input.mousePosition)))
                {
                    Interactable inter = hit.collider.GetComponent<Interactable>();
                    if (inter && holdGameObject == null)
                    {
                        inter.StartInteraction();
                        holdGameObject = inter.gameObject;
                        if (inter.Echo)
                        {
                            CmdRelayInteraction(TypeAction.START_INTERACTION, inter.Echo.gameObject);
                        }
                        break;
                    }
                }
            }

            if (Input.GetMouseButtonUp(0) && holdGameObject)
            {
                holdGameObject.GetComponent<Interactable>().EndInteraction();
                if (holdGameObject.GetComponent<Interactable>().Echo)
                {
                    CmdRelayInteraction(TypeAction.END_INTERACTION, holdGameObject.GetComponent<Interactable>().Echo.gameObject);
                }
                holdGameObject = null;
            }

            if (holdGameObject)
            {
                holdGameObject.GetComponent<Interactable>().MoveInteraction();
                if (holdGameObject.GetComponent<Interactable>().Echo)
                {
                    CmdRelayInteraction(TypeAction.MOVE_INTERACTION, holdGameObject.GetComponent<Interactable>().Echo.gameObject);
                }
            }

            #region Zoom
            //Clic droit -> zoom
            if (Input.GetMouseButtonDown(1))
            {
                if (speedZoom == 0)
                {
                    speedZoom = 20;
                }

                //On zoom
                if (speedZoom > 0)
                {
                    //On ne peut rezoomer qu'une fois etre completement dezoomé
                    if (GetComponent<Camera>().fieldOfView >= maxZoom)
                    {
                        foreach (RaycastHit hit in Physics.RaycastAll(GetComponent<Camera>().ScreenPointToRay(Input.mousePosition)))
                        {
                            if (hit.collider.tag == "Maquette")
                            {
                                target = hit.point;
                                speedZoom *= -1;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    speedZoom *= -1;
                }
            }

            if (speedZoom != 0)
            {
                float ratio = 1 - ((GetComponent<Camera>().fieldOfView - minZoom) / (maxZoom - minZoom));
                transform.LookAt(Vector3.Lerp(maquette.transform.position,target, ratio));
            }
            GetComponent<Camera>().fieldOfView = Mathf.Clamp(GetComponent<Camera>().fieldOfView + (speedZoom * Time.deltaTime),minZoom, maxZoom);
            #endregion 
        }
    }

    #region RPC et command

    [Command]
    public void CmdMove(GameObject gob, Vector3 pos)
    {
        RpcMove(gob, pos);
    }

    [ClientRpc]
    public void RpcMove(GameObject gob , Vector3 pos)
    {
        gob.transform.position = pos;
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
            transform.position = vec + deltaCam;
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

    [Command]
    void CmdRelayInteraction(TypeAction act, GameObject gob)
    {
        Player player = gob.GetComponent<Interactable>().Master.GetComponent<Player>();
        player.TargetRelay(player.GetComponent<NetworkIdentity>().connectionToClient, act, gob);
    }

    [TargetRpc]
    public void TargetRelay(NetworkConnection conn, TypeAction act, GameObject gob)
    {
        Interactable inter = gob.GetComponent<Interactable>();
        switch (act)
        {
            case TypeAction.START_INTERACTION:
                inter.StartInteraction(true);
                break;

            case TypeAction.MOVE_INTERACTION:
                inter.MoveInteraction(true);
                break;

            case TypeAction.END_INTERACTION:
                inter.EndInteraction(true);
                break;
        }
    }

    #endregion
}
