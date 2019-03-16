using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DoubleInput : NetworkBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.up, 10);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(Vector3.up, -10);
        }
    }
}
