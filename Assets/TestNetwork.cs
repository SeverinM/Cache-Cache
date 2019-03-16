using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TestNetwork : NetworkBehaviour
{
    GameObject cube;

    private void Awake()
    {
        cube = GameObject.Find("Tree");
    }

    private void Update()
    {
        if (isLocalPlayer && Input.GetMouseButton(0))
        {
             CmdRotate(cube);
        }
    }
    
    [Command]
    void CmdRotate(GameObject toRotate)
    {
        toRotate.transform.Rotate(Vector3.up, 10);
    }
}
