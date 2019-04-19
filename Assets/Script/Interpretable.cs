using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interpretable : MonoBehaviour
{
    [SerializeField]
    protected GameObject prefab;

    [System.Serializable]
    public class SpotDragAndDrop
    {
        public GameObject instance;
        public Transform where;
    }


    [SerializeField]
    List<SpotDragAndDrop> allTrsf;

    [SerializeField]
    List<AllInteractions.Actions> StartInteraction;

    [SerializeField]
    List<AllInteractions.Actions> MoveInteraction;

    [SerializeField]
    List<AllInteractions.Actions> EndInteraction;

    [SerializeField]
    int indexEcho = -1;
    public int IndexEcho => indexEcho;

    [SerializeField]
    int index = 0;
    public int Index => index;

    protected GameObject spawn;
    public Interactable Spawn => spawn.GetComponent<Interactable>();

    public virtual void Interpret(GameObject master)
    {
        if (!spawn)
        {
            spawn = Instantiate(prefab);
            Interactable inter = spawn.GetComponent<Interactable>();
            NetworkServer.SpawnWithClientAuthority(spawn, master);

            if (spawn == null) return;

            spawn.GetComponent<Interactable>().RpcTeleport(transform.position);

            foreach (AllInteractions.Actions interStart in StartInteraction)
            {
                inter.RpcAddStart(interStart);
            }

            foreach(AllInteractions.Actions interMove in MoveInteraction)
            {
                inter.RpcAddMove(interMove);
            }

            foreach(AllInteractions.Actions interStop in EndInteraction)
            {
                inter.RpcAddEnd(interStop);
            }

            foreach (SpotDragAndDrop dnd in allTrsf)
            {
                dnd.instance = Instantiate(dnd.instance);
                dnd.instance.transform.position = dnd.where.position;
                NetworkServer.SpawnWithClientAuthority(dnd.instance, master);
                inter.RpcAddSpot(dnd.where.position, dnd.instance);
            }

            inter.Master = master;
        }
    }

    public void ApplyEcho(Interpretable interpr, Player player)
    {
        Spawn.RpcSetEcho(interpr.Spawn.gameObject);
    }
}
