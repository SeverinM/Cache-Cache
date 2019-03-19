using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class ManagerGameNetwork : NetworkManager
{
    [SerializeField]
    List<GameObject> allPlayersGameObject;

    int nbPlayer = 0;

    public override void Awake()
    {
        foreach (GameObject gob in allPlayersGameObject)
        {
            ClientScene.RegisterPrefab(gob);
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn, AddPlayerMessage msg)
    {
        GameObject gob = Instantiate(allPlayersGameObject[nbPlayer]);
        gob.transform.position += Random.onUnitSphere * 2;
        NetworkServer.AddPlayerForConnection(conn, gob);
        nbPlayer++;
        gob.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, NetworkIdentity player)
    {
        base.OnServerRemovePlayer(conn, player);
        nbPlayer--;
    }
}
