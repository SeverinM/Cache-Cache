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
    public GameObject prefabSwappable;

    GameObject player1;
    GameObject player2;

    public override void OnServerAddPlayer(NetworkConnection conn, AddPlayerMessage extraMessage)
    {
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player);
        player.transform.position = (nbPlayer == 0 ? new Vector3(-1000, 0, 0) : new Vector3(1000, 0, 0));

        foreach (Player plyr in GameObject.FindObjectsOfType<Player>())
        {
            plyr.RpcUpdateCam();
        }

        nbPlayer++;

        if (player1 != null)
            player2 = player;
        else
            player1 = player;

        if (nbPlayer == 1)
        {
            GameObject gob = Instantiate(prefab1);
            gob.transform.position = player1.transform.position;
            player1.GetComponent<Player>().RpcLook(gob.transform.position);
            NetworkServer.Spawn(gob);

            GameObject objSample = Instantiate(prefabSwappable);
            objSample.transform.position = gob.transform.position + new Vector3(0, 20, 0);
            NetworkServer.SpawnWithClientAuthority(objSample, conn);
        }

        if (nbPlayer == 2)
        {
            GameObject gob2 = Instantiate(prefab2);
            gob2.transform.position = player2.transform.position;
            player2.GetComponent<Player>().RpcLook(gob2.transform.position);
            NetworkServer.Spawn(gob2);

            GameObject objSample = Instantiate(prefabSwappable);
            objSample.transform.position = gob2.transform.position + new Vector3(0, 20, 0);
            NetworkServer.SpawnWithClientAuthority(objSample, conn);

            //Initialize var
            Vector3 toTwo = player2.transform.position - player1.transform.position;
            player1.GetComponent<Player>().ToOtherPlayer = toTwo;
            player2.GetComponent<Player>().ToOtherPlayer = -toTwo;
        }
    }
}
