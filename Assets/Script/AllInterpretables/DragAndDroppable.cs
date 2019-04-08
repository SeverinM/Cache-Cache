using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DragAndDroppable : Interpretable
{
    public override void Interpret(Vector3 position, GameObject master)
    {
        GameObject gob = Instantiate(prefab,position, Quaternion.identity);
        Interactable inter = gob.GetComponent<Interactable>();
        NetworkServer.SpawnWithClientAuthority(gob, master);
        inter.RpcAddStart(0);
        inter.RpcAddMove(2);
        inter.RpcAddEnd(1);
        inter.Master = master;
    }
}
