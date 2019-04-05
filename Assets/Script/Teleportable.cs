using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Teleportable : Interpretable
{
    public override void Interpret(Vector3 position, GameObject master)
    {
        GameObject instance = Instantiate(prefab, position, Quaternion.identity);
        NetworkServer.SpawnWithClientAuthority(instance, master);
        Interactable inter = instance.GetComponent<Interactable>();
        inter.RpcAddStart(4);
        inter.Master = master;
    }
}
