using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interactable : NetworkBehaviour
{
    [SyncVar]
    public GameObject Master;

    [SerializeField]
    Material newMaterial;

    [SerializeField]
    AnimationCurve animation;

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
            _interactable = value;
        }
    }

    public delegate void InteractionDelegate (GameObject gob, GameObject master);

    public InteractionDelegate OnStart { get; set; }
    public InteractionDelegate OnEnd { get; set; }
    public InteractionDelegate OnMove { get; set; }
    public InteractionDelegate OnExit { get; set; }
    public InteractionDelegate OnEnter { get; set; }

    #region securite
    public void StartInteraction(GameObject master)
    {
        if (!CanInteract || OnStart == null)
        {
            Debug.LogWarning("Start interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        _interacting = true;
        OnStart(gameObject, master);
    }

    public void EndInteraction(GameObject master)
    {
        if (!CanInteract|| OnEnd == null)
        {
            Debug.LogWarning("End interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        _interacting = false;
        OnEnd(gameObject, master);
    }

    public void MoveInteraction(GameObject master)
    {
        if (!CanInteract || OnMove == null)
        {
            Debug.LogWarning("move interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        OnMove(gameObject, master);
    }

    public void EnterInteraction(GameObject master)
    {
        if (!CanInteract || OnEnter == null)
        {
            Debug.LogWarning("enter interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        OnEnter(gameObject, master);
    }

    public void ExitInteraction(GameObject master)
    {
        if (!CanInteract || OnExit == null)
        {
            Debug.LogWarning("exit interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        OnExit(gameObject, master);
    }

    #endregion

    private void Update()
    {
        //C'est le meme gameobject
        if (Input.GetMouseButtonUp(0) && Master.GetComponent<Player>().HoldGameObject == gameObject)
        {
            Master.GetComponent<Player>().RelayInteraction(gameObject, TypeAction.END_INTERACTION);
        }
    }

    private void OnMouseDown()
    {
        Master.GetComponent<Player>().RelayInteraction(gameObject, TypeAction.START_INTERACTION);
    }

    private void OnMouseEnter()
    {
        Master.GetComponent<Player>().RelayInteraction(gameObject, TypeAction.ENTER_INTERACTION);
    }

    private void OnMouseExit()
    {
        Master.GetComponent<Player>().RelayInteraction(gameObject, TypeAction.EXIT_INTERACTION);
    }

    public void InteractionOnServer(TypeAction act, GameObject master)
    {
        switch (act)
        {
            case TypeAction.START_INTERACTION:
                StartInteraction(master);
                break;
            case TypeAction.END_INTERACTION:
                EndInteraction(master);
                break;
            case TypeAction.EXIT_INTERACTION:
                ExitInteraction(master);
                break;
            case TypeAction.MOVE_INTERACTION:
                MoveInteraction(master);
                break;
            case TypeAction.ENTER_INTERACTION:
                EnterInteraction(master);
                break;
            default:
                Debug.LogError(act + " est inconnu");
                break;
        }
    }

    public IEnumerator Move(float timeEnd, Vector3 positionBegin , Vector3 positionEnd)
    {
        CanInteract = false;
        float time = 0;
        while (time < timeEnd)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(positionBegin, positionEnd, animation.Evaluate(time / timeEnd));
            yield return null;
        }
        CanInteract = true;
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
