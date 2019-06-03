using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleInteraction : Interactable
{
    [SerializeField]
    Animator anim;

    [SerializeField]
    string soundName = "";

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON))
        {
            if (soundName != "") Manager.GetInstance().PlaySound(mouse, soundName);
            if (!anim)
            {
                if (GetComponent<Animator>())
                    GetComponent<Animator>().SetTrigger(Manager.TRIGGER_INTERACTION);
            }               
            else
                anim.SetTrigger(Manager.TRIGGER_INTERACTION);
        }
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
    }
}
