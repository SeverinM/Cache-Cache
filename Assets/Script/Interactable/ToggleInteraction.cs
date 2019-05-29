using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleInteraction : SimpleInteraction
{
    [SerializeField]
    GameObject gobRoot;

    [SerializeField]
    ToggleInteraction otherPart;

    bool toggleState = false;
    public bool ToggleState
    {
        get
        {
            return toggleState;
        }
        set
        {
            toggleState = value;
        }
    }

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

        //Les deux parties ont toujours le meme etat
        toggleState = !toggleState;
        otherPart.ToggleState = toggleState;
    }
}
