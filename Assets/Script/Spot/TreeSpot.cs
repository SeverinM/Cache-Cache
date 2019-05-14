using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeSpot : ClassicSpot
{
    public override void ReleaseSpot(Draggable dragg)
    {
        Tree tree = (Tree)dragg;

        //it's a tree and no tree is already on the spot
        if (tree && (transform.parent == null || !transform.parent.GetComponent<Tree>()))
        {
            tree.transform.position = transform.position;
            currentHold = dragg;
            dragg.CurrentSpot = this;
        }
        else
        {
            tree.ResetPosition();
        }
    }

    public override void SetValue(Draggable dragg, bool value)
    {
        //Do nothing
    }
}
