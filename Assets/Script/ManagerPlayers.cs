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
    int occ;

    public bool HasLock(int idrequest)
    {
        if (idrequest == occ || occ == 0)
            return true;
        else
            return false;
    }

    public void AcquireLock(int newid)
    {
        occ = newid;
    }

    public void ReleaseLock()
    {
        occ = 0;
    }
}
