using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TestNetwork : NetworkBehaviour
{
    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        else
        {
            if(Input.GetKey(KeyCode.A))
            {
                Move();
            }
        }
    }

    [Command]
    void Move()
    {
        transform.position += Random.onUnitSphere * 0.01f;
    }
}
