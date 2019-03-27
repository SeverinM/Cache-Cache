using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interactable : NetworkBehaviour
{
    [SyncVar]
    public GameObject Master;

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
                CmdEndInteraction();
            }
            _interactable = value;
        }
    }

    public delegate void StartInteractionDelegate(GameObject gob, float time);
    public delegate void EndInteractionDelegate(GameObject gob);
    public delegate void MoveInteractionDelegate(GameObject gob, Vector2 delta);
    public delegate void PointerExit(GameObject gob, Vector3 position);
    public delegate void PointerEnter(GameObject gob, Vector3 position);

    public StartInteractionDelegate OnStart { get; set; }
    public EndInteractionDelegate OnEnd { get; set; }
    public MoveInteractionDelegate OnMove { get; set; }
    public PointerExit OnExit { get; set; }
    public PointerEnter OnEnter;

    public void StartInteraction(float timeStamp)
    {
        Debug.Log(OnStart);
        Debug.Log(CanInteract);
        if (!CanInteract || OnStart == null)
        {
            Debug.LogWarning("Start interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        _interacting = true;
        OnStart(gameObject, timeStamp);
    }

    [Command]
    public void CmdEndInteraction()
    {
        if (!CanInteract|| OnEnd == null)
        {
            Debug.LogWarning("End interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        _interacting = false;
        OnEnd(gameObject);
    }

    [Command]
    public void CmdMoveInteraction(Vector2 delta)
    {
        if (!CanInteract || OnEnd == null)
        {
            Debug.LogWarning("move interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        OnMove(gameObject,  delta);
    }

    [Command]
    public void CmdEnterInteraction(Vector3 position)
    {
        if (!CanInteract || OnEnter == null)
        {
            Debug.LogWarning("enter interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        OnEnter(gameObject, position);
    }

    [Command]
    public void CmdExitInteraction(Vector3 position)
    {
        if (!CanInteract || OnExit == null)
        {
            Debug.LogWarning("exit interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        OnExit(gameObject, position);
    }

    private void OnMouseDown()
    {
        Master.GetComponent<Player>().StartRelayInteraction(gameObject);
    }

    public void Teleport(Vector3 newPos)
    {
        RpcTeleport(newPos);
    }

    [ClientRpc]
    public void RpcTeleport(Vector3 newPos)
    {
        transform.position = newPos;
    }
}
