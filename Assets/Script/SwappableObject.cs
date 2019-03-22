using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SwappableObject : NetworkBehaviour
{
    public void OnMouseDown()
    {
        if (hasAuthority)
        {
            Player[] plr = GameObject.FindObjectsOfType<Player>();
            Vector3 deltaPos;
            if (plr[0].hasAuthority)
            {
                deltaPos = transform.position - plr[0].transform.position;
                transform.position = plr[1].transform.position + deltaPos;
                CmdTeleport(plr[1].transform.position + deltaPos + (Random.onUnitSphere * 3));
            }
                
            else
            {
                deltaPos = transform.position - plr[1].transform.position;
                CmdTeleport(plr[0].transform.position + deltaPos + (Random.onUnitSphere * 3));
            }

            //CmdChange(gameObject);
        }
    }

    [Command]
    void CmdTeleport(Vector3 position)
    {
        RpcTeleport(position);
    }

    [ClientRpc]
    void RpcTeleport(Vector3 position)
    {
        transform.position = position;
    }

    [Command]
    void CmdChange(GameObject gob)
    {
        Debug.Log(gob);
        Player[] plr = GameObject.FindObjectsOfType<Player>();

        if (plr.Length < 2) return;
        if (plr[0].hasAuthority)
        {
            if (gob.GetComponent<NetworkIdentity>().hasAuthority)
                gob.GetComponent<NetworkIdentity>().GetComponent<NetworkIdentity>().RemoveClientAuthority(plr[0].connectionToClient);
            GetComponent<NetworkIdentity>().AssignClientAuthority(plr[1].connectionToClient);
        }
        if (plr[1].hasAuthority)
        {
            if (gob.GetComponent<NetworkIdentity>().hasAuthority)
                gob.GetComponent<NetworkIdentity>().GetComponent<NetworkIdentity>().RemoveClientAuthority(plr[1].connectionToClient);
            GetComponent<NetworkIdentity>().AssignClientAuthority(plr[0].connectionToClient);
        }
    }
}
