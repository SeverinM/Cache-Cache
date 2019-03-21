using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkManager : NetworkManager
{
    public override void OnServerAddPlayer(NetworkConnection conn, AddPlayerMessage extraMessage)
    {
        base.OnServerAddPlayer(conn, extraMessage);
        foreach(Player plyr in GameObject.FindObjectsOfType<Player>())
        {
            plyr.RpcUpdateCam();
        }
    }
}
