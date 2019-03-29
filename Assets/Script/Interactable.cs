using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interactable : NetworkBehaviour
{
    [SyncVar]
    public GameObject Master;

    [SyncVar]
    bool grabbing = false;
    public bool Grabbing => grabbing;

    [SerializeField]
    Material newMaterial;

    public enum TypeAction
    {
        START_INTERACTION,
        END_INTERACTION,
        MOVE_INTERACTION,
        ENTER_INTERACTION,
        EXIT_INTERACTION
    }

    bool _interactable = true;
    bool _interacting = false;
    public bool Interacting
    {
        get
        {
            return _interacting;
        }
    }
    public bool CanInteract
    {
        get
        {
            return _interactable;
        }
        set
        {
            if (_interactable && !value)
            {
                EndInteraction();
            }
            _interactable = value;
        }
    }

    public delegate void StartInteractionDelegate(GameObject gob, float time);
    public delegate void EndInteractionDelegate(GameObject gob);
    public delegate void MoveInteractionDelegate(GameObject gob, Vector3 newPos);
    public delegate void PointerExit(GameObject gob, Vector3 position);
    public delegate void PointerEnter(GameObject gob, Vector3 position);

    public StartInteractionDelegate OnStart { get; set; }
    public EndInteractionDelegate OnEnd { get; set; }
    public MoveInteractionDelegate OnMove { get; set; }
    public PointerExit OnExit { get; set; }
    public PointerEnter OnEnter;

    #region securite
    public void StartInteraction(float timeStamp)
    {
        if (!CanInteract || OnStart == null)
        {
            Debug.LogWarning("Start interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        _interacting = true;
        OnStart(gameObject, timeStamp);
    }

    public void EndInteraction()
    {
        if (!CanInteract|| OnEnd == null)
        {
            Debug.LogWarning("End interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        _interacting = false;
        OnEnd(gameObject);
    }

    public void MoveInteraction(GameObject master, Vector3 pos)
    {
        if (!CanInteract || OnMove == null)
        {
            Debug.LogWarning("move interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        OnMove(gameObject, pos);
    }

    public void EnterInteraction(Vector3 position)
    {
        if (!CanInteract || OnEnter == null)
        {
            Debug.LogWarning("enter interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        OnEnter(gameObject, position);
    }

    public void ExitInteraction(Vector3 position)
    {
        if (!CanInteract || OnExit == null)
        {
            Debug.LogWarning("exit interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        OnExit(gameObject, position);
    }

    #endregion

    private void OnMouseDown()
    {
        Master.GetComponent<Player>().RelayInteraction(gameObject, TypeAction.START_INTERACTION, Master, Vector3.zero);
    }

    private void OnMouseUp()
    {
        Master.GetComponent<Player>().RelayInteraction(gameObject, TypeAction.END_INTERACTION, Master, Vector3.zero);
    }

    private void OnMouseEnter()
    {
        Master.GetComponent<Player>().RelayInteraction(gameObject, TypeAction.ENTER_INTERACTION, Master, Vector3.zero);
    }

    private void OnMouseExit()
    {
        Master.GetComponent<Player>().RelayInteraction(gameObject, TypeAction.EXIT_INTERACTION, Master, Vector3.zero);
    }

    public void InteractionOnServer(TypeAction act, GameObject master, Vector3 pos)
    {
        switch (act)
        {
            case TypeAction.START_INTERACTION:
                StartInteraction(Time.timeSinceLevelLoad);
                break;
            case TypeAction.END_INTERACTION:
                EndInteraction();
                break;
            case TypeAction.EXIT_INTERACTION:
                ExitInteraction(Random.onUnitSphere);
                break;
            case TypeAction.MOVE_INTERACTION:
                MoveInteraction(master, pos);
                break;
            case TypeAction.ENTER_INTERACTION:
                EnterInteraction(Vector2.zero);
                break;
            default:
                Debug.LogError(act + " est inconnu");
                break;
        }
    }

    public void ToggleGrab()
    {
        grabbing = !grabbing;
    }

    private void LateUpdate()
    {
        if(Input.GetMouseButtonUp(0) && hasAuthority)
        {
            grabbing = false;
        }

        if (grabbing && Master.GetComponent<NetworkIdentity>().hasAuthority)
        {
            Vector3 position = AllInteractions.GetNextPosition(transform, Master.GetComponent<Camera>());
            Master.GetComponent<Player>().RelayInteraction(gameObject, TypeAction.MOVE_INTERACTION, Master, position);
        }
    }

    #region interactionClientServeur
    [ClientRpc]
    public void RpcTeleport(Vector3 newPos)
    {
        transform.position = newPos;
    }

    [ClientRpc]
    public void RpcChangeScale(float modifier)
    {
        transform.localScale *= modifier;
    }
    #endregion
}
