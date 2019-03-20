using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Linq;

public class ManagerGameNetwork : NetworkManager
{
    [SerializeField]
    List<GameObject> allPlayersGameObject;

    [SerializeField]
    GameObject prefab;

    int nbPlayer = 0;

    public List<NetworkConnection> conns = new List<NetworkConnection>();

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
        gob.transform.position += new Vector3(10, 10, 10);
        NetworkServer.AddPlayerForConnection(conn, gob);
        nbPlayer++;
        gob.GetComponent<NetworkIdentity>().AssignClientAuthority(conn);
        conns.Add(conn);

        if (nbPlayer == 1)
        {
            GameObject gob2 = Instantiate(prefab);
            NetworkServer.SpawnWithClientAuthority(gob2, conn);
            gob2.GetComponent<SwappableObject>().CmdSetPlayer(gob);
        }
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, NetworkIdentity player)
    {
        base.OnServerRemovePlayer(conn, player);
        nbPlayer--;
        conns.Remove(conn);
    }

    public NetworkConnection GetOtherConn(NetworkConnection conn)
    {
        NetworkConnection output = null;
        if (conns.Count > 1)
        {
            output = conns.Where(x => x != conn).First();
        }
        return output;
    }

}
