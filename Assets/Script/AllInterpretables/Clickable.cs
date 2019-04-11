using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Clickable : Interpretable
{
    public override void Interpret(GameObject master)
    {
        spawn = Instantiate(prefab);
        spawn.GetComponent<Interactable>().Position = transform.position;
        Interactable inter = spawn.GetComponent<Interactable>();
        NetworkServer.SpawnWithClientAuthority(spawn, master);
        inter.Master = master;
    }
}
