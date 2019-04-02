using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interactable : NetworkBehaviour
{
    [SyncVar]
    public GameObject Master;

    [SerializeField]
    public Material newMaterial;

    [SerializeField]
    AnimationCurve animation;

    [SyncVar]
    bool spawned = false;
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

    public bool dragg = false;

    bool _canInteract = true;
    public bool CanInteract => _canInteract;

    public delegate void InteractionDelegate (GameObject gob, GameObject master, Vector3 optionalPosition);

    public InteractionDelegate OnStart { get; set; }
    public InteractionDelegate OnEnd { get; set; }
    public InteractionDelegate OnMove { get; set; }
    public InteractionDelegate OnExit { get; set; }
    public InteractionDelegate OnEnter { get; set; }

    #region securite
    public void StartInteraction(GameObject master, Vector3 position)
    {
        if (!CanInteract || OnStart == null)
        {
            Debug.LogWarning("Start interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        OnStart(gameObject, master, position);
    }

    public void EndInteraction(GameObject master, Vector3 position)
    {
        if (!CanInteract || OnEnd == null)
        {
            Debug.LogWarning("End interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        OnEnd(gameObject, master, position);
    }

    public void MoveInteraction(GameObject master, Vector3 position)
    {
        if (!CanInteract || OnMove == null)
        {
            Debug.LogWarning("move interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        OnMove(gameObject, master, position);
    }

    public void EnterInteraction(GameObject master, Vector3 position)
    {
        if (!CanInteract || OnEnter == null)
        {
            Debug.LogWarning("enter interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        OnEnter(gameObject, master, position);
    }

    public void ExitInteraction(GameObject master, Vector3 position)
    {
        if (!CanInteract || OnExit == null)
        {
            Debug.LogWarning("exit interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        OnExit(gameObject, master, position);
    }

    #endregion

    private void Awake()
    {
        OnStart = AllInteractions.START_INTERACTION;
        OnEnd = AllInteractions.END_INTERACTION;
        OnMove = AllInteractions.MOVE_INTERACTION;
    }

    private void OnMouseDown()
    {
        Interaction(TypeAction.START_INTERACTION, Master, Vector3.zero);
    }

    public void Interaction(TypeAction act, GameObject master, Vector3 position)
    {
        switch (act)
        {
            case TypeAction.START_INTERACTION:
                StartInteraction(master, position);
                break;
            case TypeAction.END_INTERACTION:
                EndInteraction(master, position);
                break;
            case TypeAction.EXIT_INTERACTION:
                ExitInteraction(master, position);
                break;
            case TypeAction.MOVE_INTERACTION:
                MoveInteraction(master, position);
                break;
            case TypeAction.ENTER_INTERACTION:
                EnterInteraction(master, position);
                break;
            default:
                Debug.LogError(act + " est inconnu");
                break;
        }
    }

    public IEnumerator Move(float timeEnd, Vector3 positionBegin , Vector3 positionEnd)
    {
        dragg = true;
        float time = 0;
        while (time < timeEnd)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(positionBegin, positionEnd, animation.Evaluate(time / timeEnd));
            yield return null;
        }
        dragg = false;
    }

    public void ToggleMat()
    {
        MeshRenderer mesh = GetComponent<MeshRenderer>();
        Material mat = mesh.material;
        mesh.material = otherMat;
        otherMat = mat;
    }
}
