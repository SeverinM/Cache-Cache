using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using UnityEngine.UI;

public class CustomNetworkManager : NetworkManager
{
    int nbPlayer = 0;
    public GameObject prefab1;
    public GameObject prefab2;

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
            maq1 = Instantiate(prefab1);
            maq1.transform.position = player.transform.position;
            player.GetComponent<Player>().RpcLook(maq1.transform.position, 0);
            NetworkServer.Spawn(maq1);
            player1 = player;
        }

        if (nbPlayer == 2)
        {
            Debug.Log("???");

            maq2 = Instantiate(prefab2);
            maq2.transform.position = player.transform.position;
            maq2.transform.parent = player.transform;
            player2 = player;

            //Interprete tous les enfants pour spawn
            foreach (Interpretable inter in maq2.transform.GetComponentsInChildren<Interpretable>())
            {
                inter.Interpret(player2);
            }

            foreach (Interpretable inter in maq1.transform.GetComponentsInChildren<Interpretable>())
            {
                inter.Interpret(player1);
            }

            //Binding des echos s'il y en a 
            List<Interpretable> allInterpr1 = maq2.GetComponentsInChildren<Interpretable>().ToList();
            List<Interpretable> allInterpr2 = maq1.GetComponentsInChildren<Interpretable>().ToList();
            foreach (Interpretable interpr in allInterpr1)
            {
                int index = interpr.IndexEcho;
                if (index >= 0)
                {
                    IEnumerable<Interpretable> listInterpr = allInterpr2.Where(x => index == x.Index);
                    if (listInterpr.Count() > 0)
                        interpr.ApplyEcho(listInterpr.ToList()[0], player1.GetComponent<Player>());
                }
            }

            foreach (Interpretable interpr in allInterpr2)
            {
                int index = interpr.IndexEcho;
                if (index >= 0)
                {
                    IEnumerable<Interpretable> listInterpr = allInterpr1.Where(x => index == x.Index);
                    if (listInterpr.Count() > 0)
                    {
                        interpr.ApplyEcho(listInterpr.ToList()[0], player2.GetComponent<Player>());
                    }                       
                }
            }


            player.GetComponent<Player>().RpcLook(maq2.transform.position, 180);
            NetworkServer.SpawnWithClientAuthority(maq2, conn);

            //Initialize var
            player1.GetComponent<Player>().RpcName("Joueur numero 1");
            player2.GetComponent<Player>().RpcName("Joueur numero 2");

            player1.GetComponent<Player>().CmdInit(maq1, instanceMan, player2);
            player2.GetComponent<Player>().CmdInit(maq2, instanceMan, player1);

            GameObject moon = player1.GetComponent<Player>().Moon;
            foreach (Draggable inter in allInterpr1.Select(x => x.Spawn.GetComponent<Draggable>()))
            {
                if (inter) inter.RpcAddSpot(moon.transform.position, moon);
            }

            GameObject moon2 = player2.GetComponent<Player>().Moon;
            foreach (Draggable inter in allInterpr2.Select(x => x.Spawn.GetComponent<Draggable>()))
            {
                if (inter) inter.RpcAddSpot(moon2.transform.position, moon2);
            }

            foreach (Interpretable interpr in allInterpr1)
            {
                Destroy(interpr.gameObject);
            }

            foreach (Interpretable interpr in allInterpr2)
            {
                Destroy(interpr.gameObject);
            }
        }
    }
}
