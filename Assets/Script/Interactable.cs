using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interactable : NetworkBehaviour
{
    [SyncVar]
    public GameObject Master;

    public Material newMaterial;
    public AnimationCurve animation;

    [SyncVar]
    bool spawned = false;

    Interactable echo;
    public Interactable Echo => echo;

    List<Spot> allSpots = new List<Spot>();
    public List<Spot> AllSpots => allSpots;
    public class Spot
    {
        public Vector3 position;
        public GameObject obj;

        public Spot(Vector3 firstPos)
        {
            position = firstPos;
            obj = null;
        }

        public void SetGob(GameObject gob, Material mat)
        {
            obj = gob;
            if (gob != null)
                obj.transform.position = position;
                obj.GetComponent<MeshRenderer>().material = mat;
        }
    }

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

    [SerializeField]
    Material otherMat;
    public Material OtherMat => otherMat;

    public IEnumerator CurrentCoroutine { get; set; }

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
    public void RpcAddSpot(Vector3 position)
    {
        allSpots.Add(new Spot(position));
    }

    public void SetAllSpots(bool value)
    {
        foreach(Spot sp in allSpots)
        {
            if (value)
            {
                sp.SetGob(AllInteractions.DuplicateVisual(gameObject), OtherMat);
            }
            else
            {
                Destroy(sp.obj);
            }
        }
    }

    private void Update()
    {
        if (Dragg)
        {
            foreach (RaycastHit hit in Physics.RaycastAll(Master.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition)))
            {
                if (hit.collider.tag == "Maquette")
                {
                    Interaction(TypeAction.MOVE_INTERACTION, hit.point);
                    break;
                }
            }
        }
    }

    public void Interaction(TypeAction act, Vector3 position, bool first = true)
    {
        GameObject master = first ? Master : Master.GetComponent<Player>().OtherPlayer;

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
            master.GetComponent<Player>().RelayInteraction(act, Echo.GetComponent<Interactable>(), position);
        }
    }

    public IEnumerator Move(float timeEnd, Vector3 positionBegin , Vector3 positionEnd)
    {
        float time = 0;
        while (time < timeEnd)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(positionBegin, positionEnd, animation.Evaluate(time / timeEnd));
            yield return null;
        }
    }

    [ClientRpc]
    public void RpcAddStart(int index)
    {
        OnStart += AllInteractions.GetDelegate(index);
    }

    [ClientRpc]
    public void RpcAddEnd(int index)
    {
        OnEnd += AllInteractions.GetDelegate(index);
    }

    [ClientRpc]
    public void RpcAddMove(int index)
    {
        OnMove += AllInteractions.GetDelegate(index);
    }

    [ClientRpc]
    public void RpcSetEcho(GameObject obj)
    {
        echo = obj.GetComponent<Interactable>();
    }

    [Command]
    public void CmdInteractionEcho(TypeAction act, GameObject gob, Vector3 position)
    {
        TargetEcho(gob.GetComponent<Interactable>().Master.GetComponent<NetworkIdentity>().connectionToClient, act, position);
    }

    [TargetRpc]
    public void TargetEcho(NetworkConnection conn, TypeAction act , Vector3 position)
    {
        GetComponent<Interactable>().Interaction(act,position, false);
    }
}
