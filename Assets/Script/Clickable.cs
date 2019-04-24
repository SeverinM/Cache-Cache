using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable : Interactable
{
    public override void StartInteraction()
    {
        GetComponent<Animator>().SetTrigger(AllInteractions.INTERACTION_TRIGGER);
    }

    public override void MoveInteraction()
    {
    }

    public override void EndInteraction()
    {
    }
}
