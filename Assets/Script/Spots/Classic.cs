using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Classic : Spot
{
    public override void CursorEnter(GameObject gob)
    {
        Debug.Log("enter");
    }

    public override void CursorExit(GameObject gob)
    {
        Debug.Log("exit");
    }

    public override void CursorRelease(GameObject gob)
    {
        Debug.Log("release");
    }

    public override void SetState(bool value)
    {
        gameObject.SetActive(value);
    }
}
