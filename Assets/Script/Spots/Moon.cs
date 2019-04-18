using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : Spot
{
    public override void CursorEnter(GameObject gob)
    {
        
    }

    public override void CursorExit(GameObject gob)
    {
        
    }

    public override void CursorRelease(GameObject gob)
    {
        AllInteractions.TELEPORT(gob, gob.GetComponent<Interactable>().Master, transform.position);
    }

    public override void SetState(bool value)
    {
    }
}
