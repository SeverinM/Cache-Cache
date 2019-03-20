using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SwappableObject : NetworkBehaviour
{
    ManagerGameNetwork manager;

    [SyncVar]
    public GameObject playerLocal;

    private void OnMouseDown()
    {
        if (hasAuthority)
        {
            CmdChangeAuthority(playerLocal);
        }
    }

    public void CmdChangeAuthority(GameObject gob)
    {
        Debug.Log("GameObject : " + gob);
        manager = GameObject.FindObjectOfType<ManagerGameNetwork>();
        NetworkConnection newConn = manager.GetOtherConn(gob.GetComponent<NetworkIdentity>().connectionToClient);
        if (newConn != null)
        {
            GetComponent<NetworkIdentity>().RemoveClientAuthority(gob.GetComponent<NetworkIdentity>().connectionToClient);
            GetComponent<NetworkIdentity>().AssignClientAuthority(newConn);
        }
    }

    public void Update()
    {
        if (hasAuthority && Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up, 10);
        }
    }

    [Command]
    public void CmdSetPlayer(GameObject gob)
    {
        playerLocal = gob;
    }
}
