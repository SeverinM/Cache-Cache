using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Draggable
{
    [HideInInspector]
    public Squirrel squirrel;

    private void Awake()
    {
        squirrel = null;
    }

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (squirrel)
        {
            squirrel.NextJump();
        }
        else
        {
            base.MouseDown(btn, mouse, echo);
        }
    }

    public override void MouseEnter(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        base.MouseEnter(btn, mouse, echo);
    }

    public override void MouseLeave(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        base.MouseLeave(btn, mouse, echo);
    }

    public override void MouseMove(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        base.MouseMove(btn, mouse, echo);
    }

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        base.MouseUp(btn, mouse, echo);
    }

    public override void OnNewValue()
    {
        base.OnNewValue();
    }
}
