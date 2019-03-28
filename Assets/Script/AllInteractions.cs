using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AllInteractions
{
    public static void ENTER_INTERACTION(GameObject gob, Vector3 pos)
    {
        gob.GetComponent<Interactable>().RpcChangeScale(2f);
    }

    public static void EXIT_INTERACTION(GameObject gob , Vector3 position)
    {
        gob.GetComponent<Interactable>().RpcChangeScale(0.5f);
    }

    public static void START_INTERACTION(GameObject gob, float timeStamp)
    {
        gob.GetComponent<Interactable>().RpcTeleport(gob.transform.position + new Vector3(0, 1, 0));
    }

    public static void MOVE_INTERACTION(GameObject gob, Vector2 motion)
    {

    }

    public static void END_INTERACTION(GameObject gob)
    {

    }
}
