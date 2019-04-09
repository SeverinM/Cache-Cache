using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DragAndDroppable : Interpretable
{
    public override void Interpret(GameObject master)
    {
        if (!spawn)
        {
            spawn = Instantiate(prefab, transform.position, Quaternion.identity);
            Interactable inter = spawn.GetComponent<Interactable>();
            NetworkServer.SpawnWithClientAuthority(spawn, master);
            inter.RpcAddStart(0);
            inter.RpcAddMove(2);
            inter.RpcAddEnd(1);
            inter.Master = master;
        }
    }
}
