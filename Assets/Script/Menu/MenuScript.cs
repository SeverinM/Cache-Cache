using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : Interactable
{
    [SerializeField]
    Panel toDisappear;

    [SerializeField]
    Panel toAppear;

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
    }

    public override void MouseEnter(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        
    }

    public override void MouseLeave(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        
    }

    public override void MouseMove(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        
    }

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (btn == MouseInputManager.MouseButton.LEFT_BUTTON && canInteract)
        {
            toDisappear.SetValue(false);
            toAppear.SetValue(true);
        }
    }
}
