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

    [SerializeField]
    Vector3 deltaCam = new Vector3(100, 50, 0);

    [SyncVar]
    GameObject otherPlayer;
    public GameObject OtherPlayer => otherPlayer;

    [SyncVar]
    GameObject man;
    
    //Zoom
    IEnumerator zoomCoroutine;
    Vector3 focusZoom;
    Vector3 previousForward;
    float lastFOV;
    bool zoomed = false;

    [SerializeField]
    AnimationCurve curve;

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

            //Clic droit appuyé
            if (Input.GetMouseButtonDown(1))
            {
                RaycastHit hit;
                //On zoom sur la premiere collision trouvé
                if (Physics.Raycast(GetComponent<Camera>().ScreenPointToRay(Input.mousePosition),out hit))
                {
                    zoomed = true;
                    if (zoomCoroutine != null)
                    {
                        StopCoroutine(zoomCoroutine);
                        GetComponent<Camera>().fieldOfView = lastFOV;
                    }

                    if (zoomed)
                    {
                        zoomCoroutine = InterpolateZoom(1,(focusZoom - transform.position).normalized, previousForward, -30);
                    }
                    else
                    {
                        zoomCoroutine = InterpolateZoom(1, transform.forward, (hit.point - transform.position).normalized, 30);
                    }
                    
                    StartCoroutine(zoomCoroutine);
                    lastFOV = GetComponent<Camera>().fieldOfView - 30;
                    previousForward = transform.forward;
                    focusZoom = hit.point;
                    zoomed = !zoomed;
                }
            }
        }
    }

    IEnumerator InterpolateZoom(float duration, Vector3 initialForward, Vector3 finalForward, float modifierFOV)
    {
        float time = 0;
        float ratio = 0;
        float firstFOV = GetComponent<Camera>().fieldOfView;
        while (time < duration)
        {
            time += Time.deltaTime;
            ratio = curve.Evaluate(time / duration);
            transform.forward = Vector3.Lerp(initialForward, finalForward, ratio);
            GetComponent<Camera>().fieldOfView = firstFOV + (ratio * modifierFOV);
            yield return 1;
        }
        zoomCoroutine = null;
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
