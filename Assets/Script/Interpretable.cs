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
            spawn.transform.position = transform.position;
            NetworkServer.SpawnWithClientAuthority(spawn, master);

            if (spawn == null || inter == null)
            {
                return;
            }
            inter.Master = master;

            Draggable dragg = spawn.GetComponent<Draggable>();
            if (!dragg) return;
            foreach (SpotDragAndDrop dnd in allTrsf)
            {
                dnd.instance = Instantiate(dnd.instance);
                dnd.instance.transform.position = dnd.where.position;
                NetworkServer.SpawnWithClientAuthority(dnd.instance, master);
                dragg.RpcAddSpot(dnd.where.position, dnd.instance);
            }
        }
    }

    public void ApplyEcho(Interpretable interpr, Player player)
    {
        Spawn.RpcSetEcho(interpr.Spawn.gameObject);
    }
}
