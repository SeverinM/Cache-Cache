using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interactable : NetworkBehaviour
{
    class SpotDragAndDropInter
    {
        public Vector3 position;
        public GameObject instance;
        public SpotDragAndDropInter(Vector3 firstPosition , GameObject firstInst)
        {
            position = firstPosition;
            instance = firstInst;
        }
    }

    [HideInInspector]
    [SyncVar]
    public GameObject Master;

    [SyncVar]
    bool spawned = false;

    public Spot currentSpot;
    public Vector3 startPosition;

    Interactable echo;
    public Interactable Echo => echo;

    List<SpotDragAndDropInter> allSpots = new List<SpotDragAndDropInter>();
    List<SpotDragAndDropInter> AllSpots => allSpots;

    public bool Spawned
    {
        get
        {
            return spawned;
        }
        set
        {
            if (!value)
            {
                Debug.LogError("impossible de mettre a false");
            }
            spawned = true;
        }
    }

    [HideInInspector]
    [SerializeField]
    Material otherMat;
    public Material OtherMat => otherMat;

    public enum TypeAction
    {
        START_INTERACTION,
        END_INTERACTION,
        MOVE_INTERACTION,
        ENTER_INTERACTION,
        EXIT_INTERACTION
    }

    bool _canInteract = true;
    public bool CanInteract => _canInteract;

    [HideInInspector]
    public bool Dragg = false;

    public delegate void InteractionDelegate (GameObject gob, GameObject master, Vector3 optionalPosition);

    public InteractionDelegate OnStart { get; set; }
    public InteractionDelegate OnEnd { get; set; }
    public InteractionDelegate OnMove { get; set; }
    public InteractionDelegate OnExit { get; set; }
    public InteractionDelegate OnEnter { get; set; }

    #region securite
    public void StartInteraction(GameObject master, Vector3 position)
    {
        if (OnStart == null)
        {
            Debug.LogWarning("Start interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        OnStart(gameObject, master, position);
    }

    public void EndInteraction(GameObject master, Vector3 position)
    {
        if (OnEnd == null)
        {
            Debug.LogWarning("End interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        OnEnd(gameObject, master, position);
    }

    public void MoveInteraction(GameObject master, Vector3 position)
    {
        if (OnMove == null)
        {
            Debug.LogWarning("move interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        OnMove(gameObject, master, position);
    }

    public void EnterInteraction(GameObject master, Vector3 position)
    {
        if (OnEnter == null)
        {
            Debug.LogWarning("enter interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        OnEnter(gameObject, master, position);
    }

    public void ExitInteraction(GameObject master, Vector3 position)
    {
        if (OnExit == null)
        {
            Debug.LogWarning("exit interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        OnExit(gameObject, master, position);
    }

    #endregion

    [ClientRpc]
    public void RpcAddSpot(Vector3 position, GameObject gob)
    {
        Debug.Log(gob);
        allSpots.Add(new SpotDragAndDropInter(position, gob));
        gob.GetComponent<Spot>().SetState(false);
    }

    [ClientRpc]
    public void RpcTeleport(Vector3 position)
    {
        transform.position = position;
    }

    public void SetAllSpots(bool value)
    {
        foreach (SpotDragAndDropInter sp in allSpots)
        {
            sp.instance.GetComponent<Spot>().SetState(value);
        }
    }

    private void Update()
    {
        if (Dragg)
        {
            foreach (RaycastHit hit in Physics.RaycastAll(Master.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition)))
            {
                if (hit.collider.tag == "Maquette" || hit.collider.tag == "Rallonge")
                {
                    Interaction(TypeAction.MOVE_INTERACTION, hit.point);
                    break;
                }
            }
        }
    }

    public void Interaction(TypeAction act, Vector3 position, bool first = true)
    {
        GameObject master = Master;

        if (!CanInteract) Debug.LogWarning("L'objet n'est pas interactible pour le moment");
        if (!master.GetComponent<NetworkIdentity>().hasAuthority) Debug.LogWarning("Pas d'autorité , echo " + !first);

        switch (act)
        {

            case TypeAction.START_INTERACTION:
                StartInteraction(Master, position);
                break;
            case TypeAction.END_INTERACTION:
                EndInteraction(Master, position);
                break;
            case TypeAction.EXIT_INTERACTION:
                ExitInteraction(Master, position);
                break;
            case TypeAction.MOVE_INTERACTION:
                MoveInteraction(Master, position);
                break;
            case TypeAction.ENTER_INTERACTION:
                EnterInteraction(Master, position);
                break;
            default:
                Debug.LogError(act + " est inconnu");
                break;
        }

        if (Echo && first)
        {
            CmdInteractionEcho(act, gameObject, position);
        }
    }

    [ClientRpc]
    public void RpcAddStart(AllInteractions.Actions act)
    {
        OnStart += AllInteractions.GetDelegate(act);
    }

    [ClientRpc]
    public void RpcAddEnd(AllInteractions.Actions act)
    {
        OnEnd += AllInteractions.GetDelegate(act);
    }

    [ClientRpc]
    public void RpcAddMove(AllInteractions.Actions act)
    {
        OnMove += AllInteractions.GetDelegate(act);
    }

    [ClientRpc]
    public void RpcSetEcho(GameObject obj)
    {
        echo = obj.GetComponent<Interactable>();
    }

    [Command]
    public void CmdInteractionEcho(TypeAction act, GameObject gob, Vector3 position)
    {
        Interactable ech = gob.GetComponent<Interactable>().Echo;
        Debug.Log(ech);
        ech.TargetEcho(ech.Master.GetComponent<NetworkIdentity>().connectionToClient, act, position);
    }

    [TargetRpc]
    public void TargetEcho(NetworkConnection conn, TypeAction act , Vector3 position)
    {
        GetComponent<Interactable>().Interaction(act,position, false);
    }
}
