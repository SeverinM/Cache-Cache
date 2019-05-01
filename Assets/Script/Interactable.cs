using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public abstract class Interactable : NetworkBehaviour
{
    [HideInInspector]
    [SyncVar]
    public GameObject Master;

    public abstract void StartInteraction(bool asEcho = false);
    public abstract void MoveInteraction(bool asEcho = false);
    public abstract void EndInteraction(bool asEcho = false);

    protected Interactable echo;
    public Interactable Echo => echo;

    public void Teleport()
    {
        Player plr = Master.GetComponent<Player>();
        Player other = plr.OtherPlayer.GetComponent<Player>();
        Vector3 delta = (other.maquette.transform.position - plr.maquette.transform.position);
        plr.CmdMove(gameObject, transform.position + (other.maquette.transform.position - plr.maquette.transform.position));
        plr.CmdChangeAuthority(gameObject, plr.gameObject, plr.OtherPlayer);
    }

    #region Rpc et Cmd

    [ClientRpc]
    public void RpcTeleport(Vector3 position)
    {
        transform.position = position;
    }

    [ClientRpc]
    public void RpcSetEcho(GameObject obj)
    {
        echo = obj.GetComponent<Interactable>();
    }
    #endregion
}
