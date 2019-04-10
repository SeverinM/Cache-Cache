using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class Interpretable : MonoBehaviour
{
    [SerializeField]
    protected GameObject prefab;

    [SerializeField]
    int indexEcho = -1;
    public int IndexEcho => indexEcho;

    [SerializeField]
    int index = 0;
    public int Index => index;

    protected GameObject spawn;
    public Interactable Spawn => spawn.GetComponent<Interactable>();

    public abstract void Interpret(GameObject gob);

    public void ApplyEcho(Interpretable interpr, Player player)
    {
        Spawn.RpcSetEcho(interpr.Spawn.gameObject);
    }
}
