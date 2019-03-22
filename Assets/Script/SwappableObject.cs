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
            }
                
            else
            {
                deltaPos = transform.position - plr[1].transform.position;
                transform.position = plr[0].transform.position + deltaPos;
            }

            //CmdChange(gameObject);
        }
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
