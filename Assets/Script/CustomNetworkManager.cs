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

    public override void OnServerAddPlayer(NetworkConnection conn, AddPlayerMessage extraMessage)
    {
        GameObject player = Instantiate(playerPrefab, GetStartPosition().transform.position,Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player);

        foreach(Player plyr in GameObject.FindObjectsOfType<Player>())
        {
            plyr.RpcUpdateCam();
        }

        player.GetComponent<Player>().RpcSetMaquette(nbPlayer == 0 ? prefab1 : prefab2);
        nbPlayer++;

        if (nbPlayer == 2)
        {

            List<Player> allGobs = GameObject.FindObjectsOfType<Player>().OrderBy(x => x.id).ToList();

            GameObject gob = Instantiate(prefab1);
            gob.transform.position = allGobs[0].transform.position + new Vector3(0, 20, 0);
            NetworkServer.Spawn(gob);

            GameObject gob2 = Instantiate(prefab2);
            gob2.transform.position = allGobs[1].transform.position + new Vector3(0, 20, 0);
            NetworkServer.Spawn(gob2);
        }
    }
}
