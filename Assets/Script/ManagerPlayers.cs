using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public enum Occupation
{
    NOTHING,
    PLAYER_1,
    PLAYER_2
}

public class ManagerPlayers : NetworkBehaviour
{
    [SyncVar]
    public Occupation rotate = Occupation.NOTHING;

    public bool HasLock(Occupation occ)
    {
        Debug.Log(rotate);
        if (occ == rotate || rotate == Occupation.NOTHING)
            return true;
        else
            return false;
    }

    [Command]
    public void CmdAcquireLock(Occupation occ)
    {
        rotate = occ;
    }

    [Command]
    public void CmdReleaseLock()
    {
        rotate = Occupation.NOTHING;
    }
}
