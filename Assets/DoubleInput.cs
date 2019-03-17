using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DoubleInput : NetworkBehaviour
{
    int count = 0;
    int maxCount = 10;
    // Update is called once per frame
    void Update()
    {
        count++;
        if (maxCount == count)
        {
            count = 0;
        }        
    }

}
