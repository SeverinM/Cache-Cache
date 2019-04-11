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
    public GameObject prefabSwappable;

    [SerializeField]
    List<Button> allButtons;

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
            NetworkServer.SpawnWithClientAuthority(maq1, conn);
            player1 = player;
            //player1.GetComponent<Player>().CanRotate = false;
        }

        if (nbPlayer == 2)
        {
            maq2 = Instantiate(prefab2);
            maq2.transform.position = player.transform.position;
            maq2.transform.parent = player.transform;
            player2 = player;

            //Interprete tous les enfants pour spawn
            foreach (Transform trsf in maq2.transform)
            {
                trsf.GetComponent<Interpretable>().Interpret(player2);
            }

            foreach (Transform trsf in maq1.transform)
            {
                trsf.GetComponent<Interpretable>().Interpret(player1);
            }

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
            Vector3 toTwo = player2.transform.position - player1.transform.position;
            player1.GetComponent<Player>().ToOtherPlayer = toTwo;
            player2.GetComponent<Player>().ToOtherPlayer = -toTwo;
            player1.GetComponent<Player>().RpcName("Joueur numero 1");
            player2.GetComponent<Player>().RpcName("Joueur numero 2");

            player1.GetComponent<Player>().CmdInit(maq1, instanceMan, player2);
            player2.GetComponent<Player>().CmdInit(maq2, instanceMan, player1);
        }
    }
}
