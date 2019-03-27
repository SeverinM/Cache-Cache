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
    public GameObject managerPrefab;

    GameObject player1;
    GameObject player2;

    GameObject maq1;
    GameObject maq2;

    GameObject instanceMan;

    public override void OnServerAddPlayer(NetworkConnection conn, AddPlayerMessage extraMessage)
    {
        GameObject player = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, player);
        player.transform.position = (nbPlayer == 0 ? new Vector3(-1000, 0, 0) : new Vector3(1000, 0, 0));
        player.name = "Joueur numero " + nbPlayer;

        foreach (Player plyr in GameObject.FindObjectsOfType<Player>())
        {
            plyr.RpcUpdateCam();
        }

        nbPlayer++;
        if (nbPlayer == 1)
        {
            instanceMan = Instantiate(managerPrefab);
            NetworkServer.Spawn(instanceMan);
            maq1 = Instantiate(prefab1);
            maq1.transform.position = player.transform.position;
            player.GetComponent<Player>().RpcLook(maq1.transform.position, 0);
            NetworkServer.SpawnWithClientAuthority(maq1, conn);

            GameObject objSample = Instantiate(prefabSwappable);
            objSample.transform.position = maq1.transform.position + new Vector3(0, 20, 0);
            objSample.GetComponent<Interactable>().OnStart = StartInteraction;
            objSample.GetComponent<Interactable>().Master = player;
            NetworkServer.Spawn(objSample);

            player1 = player;            
        }

        if (nbPlayer == 2)
        {
            maq2 = Instantiate(prefab2);
            maq2.transform.position = player.transform.position;
            maq2.transform.parent = player.transform;
            player.GetComponent<Player>().RpcLook(maq2.transform.position, 180);
            NetworkServer.SpawnWithClientAuthority(maq2, conn);

            GameObject objSample = Instantiate(prefabSwappable);
            objSample.GetComponent<Interactable>().OnStart = StartInteraction;
            objSample.GetComponent<Interactable>().Master = player;
            objSample.transform.position = maq2.transform.position + new Vector3(0, 20, 0);
            NetworkServer.Spawn(objSample);

            player2 = player;

            //Initialize var
            Vector3 toTwo = player2.transform.position - player1.transform.position;
            player1.GetComponent<Player>().ToOtherPlayer = toTwo;
            player2.GetComponent<Player>().ToOtherPlayer = -toTwo;

            player1.GetComponent<Player>().CmdInit(1, maq1, instanceMan, player2);
            player2.GetComponent<Player>().CmdInit(2, maq2, instanceMan, player1);
        }
    }

    public void StartInteraction(GameObject gob, float timestamp)
    {
        gob.GetComponent<Interactable>().Teleport(gob.transform.position + new Vector3(0, 10, 0));
    }
}
