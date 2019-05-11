using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : Interactable
{
    [SerializeField]
    GameObject begin;

    [SerializeField]
    GameObject end;

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
        if (btn == MouseInputManager.MouseButton.LEFT_BUTTON)
        {
            end.SetActive(true);
            begin.SetActive(false);
        }
    }
}
