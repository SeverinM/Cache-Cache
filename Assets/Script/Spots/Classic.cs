using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Classic : Spot
{
    public override void CursorEnter()
    {
        Debug.Log("enter");
    }

    public override void CursorExit()
    {
        Debug.Log("exit");
    }

    public override void CursorRelease()
    {
        Debug.Log("release");
    }
}
