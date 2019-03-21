using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Player : NetworkBehaviour
{
    public static int count = 0;

    public void Start()
    {
        if (isLocalPlayer)
        {
            Utils.DisableAllOthersCam(GetComponent<Camera>());
            transform.LookAt(new Vector3(0, 0, 0));
            count++;
            GameObject.FindObjectOfType<Text>().text = count.ToString();
        }       
    }
}
