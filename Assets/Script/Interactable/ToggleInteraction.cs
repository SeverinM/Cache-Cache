using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleInteraction : SimpleInteraction
{
    [SerializeField]
    GameObject gobRoot;

    [SerializeField]
    ToggleInteraction otherPart;

    [SerializeField]
    float coolDown = 1;

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
        //Anti echo
        if (!CanInteract) return;
        
        StartCoroutine(StartCoolDown());
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

    IEnumerator StartCoolDown()
    {
        CanInteract = false;
        otherPart.CanInteract = false;
        yield return new WaitForSeconds(coolDown);
        CanInteract = true;
        otherPart.CanInteract = true;
    }
}
