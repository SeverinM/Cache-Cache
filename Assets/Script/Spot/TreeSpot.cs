using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpot : ClassicSpot
{
    public override void ReleaseSpot(Draggable dragg)
    {
        //it's a tree and no tree is already on the spot
        if (dragg && (transform.parent == null || !transform.parent.GetComponent<Tree>()))
        {
            dragg.transform.position = transform.position;
            currentHold = dragg;
            dragg.CurrentSpot = this;
            dragg.transform.SetParent(this.transform);
            dragg.transform.localRotation = Quaternion.identity;
        }
        else
        {
            dragg.ResetPosition();
        }
    }

    public override void PressSpot(Draggable dragg)
    {
        base.PressSpot(dragg);
        //transform.parent = null;
    }

    public override void ResetSpot(Draggable dragg)
    {
        dragg.transform.SetParent(this.transform);
        base.ResetSpot(dragg);
    }

    public override void SetValue(Draggable dragg, bool value)
    {
        //Do nothing
    }
}
