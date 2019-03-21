using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class CustomNetworkManager : NetworkManager
{
    int nbPlayer = 0;
    public GameObject prefab1;
    public GameObject prefab2;

    GameObject player1;
    GameObject player2;

    public override void OnServerAddPlayer(NetworkConnection conn, AddPlayerMessage extraMessage)
    {
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player);
        player.transform.position += Random.onUnitSphere * 1000;

        foreach (Player plyr in GameObject.FindObjectsOfType<Player>())
        {
            plyr.RpcUpdateCam();
        }

        nbPlayer++;

        if (player1 != null)
            player2 = player;
        else
            player1 = player;

        if (nbPlayer == 2)
        {
            GameObject gob = Instantiate(prefab1);
            gob.transform.position = player1.transform.position + new Vector3(0, 20, 0);
            player1.GetComponent<Player>().RpcLook(gob.transform.position);
            NetworkServer.Spawn(gob);

            GameObject gob2 = Instantiate(prefab2);
            gob2.transform.position = player2.transform.position + new Vector3(0, 20, 0);
            player2.GetComponent<Player>().RpcLook(gob2.transform.position);
            NetworkServer.Spawn(gob2);
        }
    }
}
