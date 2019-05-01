using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Classic : Spot
{
    public override void CursorEnter(GameObject gob)
    {

    }

    public override void CursorExit(GameObject gob)
    {
    }

    public override void CursorRelease(GameObject gob)
    {
        gob.transform.position = transform.position;
    }

    public override void SetState(bool value)
    {
        gameObject.SetActive(value);
    }
}
