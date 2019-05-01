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
    public GameObject textEnd;

    GameObject player1;
    GameObject player2;

    GameObject maq1;
    GameObject maq2;

    GameObject instanceMan;

    NetworkConnection conn1;
    NetworkConnection conn2;

    [SerializeField]
    Canvas canvas;

    public override void OnServerAddPlayer(NetworkConnection conn, AddPlayerMessage extraMessage)
    {
        if (nbPlayer == 0)
        {
            conn1 = conn;
        }
        else
        {
            conn2 = conn;
        }


        nbPlayer++;

        if (nbPlayer == 2)
        {
            TwoPlayerConnected();
        }
    }

    public void TwoPlayerConnected()
    {
        Destroy(canvas.gameObject);
        GameObject player1 = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn1, player1);
        player1.transform.position = new Vector3(-1000, 0, 0);
        maq1 = Instantiate(prefab1);
        maq1.transform.position = player1.transform.position;
        player1.GetComponent<Player>().RpcUpdateCam();
        player1.GetComponent<Player>().RpcLook(maq1.transform.position, 0);
        NetworkServer.Spawn(maq1);

        
        //=====
        GameObject player2 = Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn2, player2);
        player2.transform.position = new Vector3(1000, 0, 0);
        player2.GetComponent<Player>().RpcUpdateCam();
        maq2 = Instantiate(prefab2);
        maq2.transform.position = player2.transform.position;
        maq2.transform.parent = player2.transform;
        player2.GetComponent<Player>().RpcLook(maq2.transform.position, 0);
        //===

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

        player2.GetComponent<Player>().RpcLook(maq2.transform.position, 180);
        NetworkServer.SpawnWithClientAuthority(maq2, conn2);

        //Initialize var

        player1.GetComponent<Player>().CmdInit(maq1, instanceMan, player2);
        player2.GetComponent<Player>().CmdInit(maq2, instanceMan, player1);

        foreach (Interpretable interpr in allInterpr1)
        {
            Destroy(interpr.gameObject);
        }

        foreach (Interpretable interpr in allInterpr2)
        {
            Destroy(interpr.gameObject);
        }
    }

    //Le client a quitté
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        textEnd.SetActive(true);
        player1.GetComponent<Player>().CanInteract = false;
        player2.GetComponent<Player>().CanInteract = false;
        base.OnClientDisconnect(conn);
    }

    //L'hote a quitté
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        textEnd.SetActive(true);
        player1.GetComponent<Player>().CanInteract = false;
        player2.GetComponent<Player>().CanInteract = false;
        base.OnServerDisconnect(conn);
    }
}
