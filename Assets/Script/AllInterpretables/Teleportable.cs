using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Teleportable : Interpretable
{
    public override void Interpret(GameObject master)
    {
        if (!spawn)
        {
            spawn = Instantiate(prefab, transform.position, Quaternion.identity);
            NetworkServer.SpawnWithClientAuthority(spawn, master);
            Interactable inter = spawn.GetComponent<Interactable>();
            inter.RpcAddStart(4);
            inter.Master = master;
        }       
    }
}
