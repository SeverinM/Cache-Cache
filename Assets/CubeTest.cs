using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeTest : Interactable
{
    public override void MouseDown(int id)
    {
        Debug.LogError("Cube");
    }

    public override void MouseEnter(int id)
    {
        throw new System.NotImplementedException();
    }

    public override void MouseLeave(int id)
    {
        throw new System.NotImplementedException();
    }

    public override void MouseMove(int id, Vector2 delta)
    {
        throw new System.NotImplementedException();
    }

    public override void MouseUp(int id)
    {
        throw new System.NotImplementedException();
    }
}
