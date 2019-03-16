using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TestNetwork : NetworkBehaviour
{
    [SyncVar]
    Vector3 position = new Vector3();

    private void Awake()
    {
        position = transform.position;
    }

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
                position = transform.position + Random.onUnitSphere * 0.1f;
            }
        }

        transform.position = position;
    }
}
