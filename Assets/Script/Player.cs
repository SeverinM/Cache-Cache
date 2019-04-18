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

    [SerializeField]
    AnimationCurve curve;

    [SerializeField]
    GameObject moon;
    public GameObject Moon => moon;

    //Utilisé dans le drag and drop
    public GameObject holdGameObject;
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
    Vector3 source;

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

            //Clic droit 
            if (Input.GetMouseButtonDown(1))
            {
                if (speedZoom == 0)
                {
                    speedZoom = 20;
                }

                //On zoom
                if (speedZoom > 0)
                {
                    foreach (RaycastHit hit in Physics.RaycastAll(GetComponent<Camera>().ScreenPointToRay(Input.mousePosition)))
                    {
                        if (hit.collider.tag == "Maquette")
                        {
                            target = (hit.point - transform.position).normalized - ForwardMaquette;
                            source = Vector3.zero;
                            Debug.DrawRay(transform.position + transform.forward, target * 100, Color.green, 100);

                            speedZoom *= -1;
                            break;
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
                float ratio = (GetComponent<Camera>().fieldOfView - minZoom) / (maxZoom - minZoom);
                transform.forward = Vector3.Lerp(ForwardMaquette + source, ForwardMaquette + target, ratio);
            }
            transform.forward = ForwardMaquette;
            GetComponent<Camera>().fieldOfView = Mathf.Clamp(GetComponent<Camera>().fieldOfView + (speedZoom * Time.deltaTime),minZoom, maxZoom);
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
            transform.position = vec + new Vector3(100, 50, 0);
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

        moon = Instantiate(moon);

        float width = GetComponent<Camera>().pixelWidth;
        float height = GetComponent<Camera>().pixelHeight;
        moon.transform.position = maq.transform.position + new Vector3(0, 40, 0);

        NetworkServer.SpawnWithClientAuthority(moon, gameObject);
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
