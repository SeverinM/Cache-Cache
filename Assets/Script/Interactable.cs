using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interactable : NetworkBehaviour
{
    public GameObject Master { get; set; }

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
    public delegate void MoveInteractionDelegate(GameObject gob, Vector2 delta);
    public delegate void PointerExit(GameObject gob, Vector3 position);
    public delegate void PointerEnter(GameObject gob, Vector3 position);

    public StartInteractionDelegate OnStart { get; set; }
    public EndInteractionDelegate OnEnd { get; set; }
    public MoveInteractionDelegate OnMove { get; set; }
    public PointerExit OnExit { get; set; }
    public PointerEnter OnEnter { get; set; }

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

    public void MoveInteraction(Vector2 delta)
    {
        if (!CanInteract || OnEnd == null)
        {
            Debug.LogWarning("move interaction a echoué : objet non interactible ou pas de delegate");
            return;
        }
        OnMove(gameObject,  delta);
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

    private void OnMouseDown()
    {
        Debug.Log("debut interaction");
        StartInteraction(Time.timeSinceLevelLoad);
    }
}
