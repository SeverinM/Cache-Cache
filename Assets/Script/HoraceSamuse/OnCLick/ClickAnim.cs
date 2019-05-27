using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickAnim : Interactable
{
    public Animator animator;
    public string triggerName = "Clique";
    
    [ContextMenu("Touche me")]
    public void Touch()
    {
        animator.SetTrigger(triggerName);
    }

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null) { }
    public override void MouseEnter(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null) { }
    public override void MouseLeave(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null) { }
    public override void MouseMove(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null) { }

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        //
        Debug.Log("not me");
        if (btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON))
            Touch();
    }
}
