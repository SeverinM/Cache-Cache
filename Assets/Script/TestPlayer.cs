using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TestPlayer : NetworkBehaviour
{
    private void Update()
    {
        if (isLocalPlayer && Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.up, 10);
        }
    }

    public override bool OnCheckObserver(NetworkConnection conn)
    {
        Debug.Log("kop");
        if (!hasAuthority)
            return false;
        return true;
    }
}
