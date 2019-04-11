using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DragAndDroppable : Interpretable
{
    [SerializeField]
    List<Transform> allTrsf;

    public override void Interpret(GameObject master)
    {
        if (!spawn)
        {
            spawn = Instantiate(prefab);
            spawn.GetComponent<Interactable>().Position = transform.position;
            Interactable inter = spawn.GetComponent<Interactable>();
            NetworkServer.SpawnWithClientAuthority(spawn, master);
            inter.RpcAddStart(0);
            inter.RpcAddMove(2);
            inter.RpcAddEnd(1);
            inter.Master = master;

            foreach(Transform trsf in allTrsf)
            {
                inter.RpcAddSpot(trsf.position);
            }
        }
    }
}
