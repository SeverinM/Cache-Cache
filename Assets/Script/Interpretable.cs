using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class Interpretable : MonoBehaviour
{
    [SerializeField]
    protected GameObject prefab;

    [System.Serializable]
    public class SpotDragAndDrop
    {
        public GameObject prefab;
        public Transform where;
        public bool ApplyEcho;
        public bool ApplyItself;

        [HideInInspector]
        public GameObject instance = null;
    }

    [SerializeField]
    List<SpotDragAndDrop> allTrsf;

    [SerializeField]
    [Tooltip("Si mis a vrai , les deux objets ne vont pas imiter leur comportement")]
    bool echoOnlySpot = true;

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
            foreach (SpotDragAndDrop dnd in allTrsf.Where( x => x.ApplyItself))
            {
                if (!dnd.instance)
                {
                    dnd.instance = Instantiate(dnd.prefab);
                    dnd.instance.transform.position = dnd.where.position;
                }
                
                NetworkServer.SpawnWithClientAuthority(dnd.instance, master);
                dragg.RpcAddSpot(dnd.where.position, dnd.instance);
            }
        }
    }

    public void ApplyEcho(Interpretable interpr, Player player)
    {
        if (!echoOnlySpot)
            Spawn.RpcSetEcho(interpr.Spawn.gameObject);

        foreach (SpotDragAndDrop dnd in allTrsf.Where(x => x.ApplyEcho))
        {
            if (!dnd.instance)
            {
                dnd.instance = Instantiate(dnd.prefab);
                dnd.instance.transform.position = dnd.where.position;
            }
            
            NetworkServer.SpawnWithClientAuthority(dnd.instance, player.gameObject);
            if (interpr.Spawn.GetComponent<Draggable>())
                interpr.Spawn.GetComponent<Draggable>().RpcAddSpot(dnd.where.position, dnd.instance);
        }
    }
}
