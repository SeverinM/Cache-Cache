using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleInteraction : SimpleInteraction
{
    [SerializeField]
    GameObject gobRoot;

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        base.MouseDown(btn, mouse, echo);

        if (gobRoot)
        {
            foreach (Interactable inter in gobRoot.GetComponentsInChildren<Draggable>())
            {
                inter.CanInteract = !inter.CanInteract;
            }
        }       
    }
}
