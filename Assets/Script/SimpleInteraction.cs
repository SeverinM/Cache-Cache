using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleInteraction : Interactable
{
    public override void MouseDown(MouseInputManager.MouseButton btn, Interactable echo = null)
    {
        if (btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON))
        {
            GetComponent<Animator>().SetTrigger(Manager.TRIGGER_INTERACTION);
        }
    }

    public override void MouseEnter(MouseInputManager.MouseButton btn, Interactable echo = null)
    {
    }

    public override void MouseLeave(MouseInputManager.MouseButton btn, Interactable echo = null)
    {
    }

    public override void MouseMove(MouseInputManager.MouseButton btn, Vector2 delta, Interactable echo = null)
    {
    }

    public override void MouseUp(MouseInputManager.MouseButton btn, Interactable echo = null)
    {
    }
}
