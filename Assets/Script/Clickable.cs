using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable : Interactable
{
    public override void StartInteraction(bool asEcho)
    {
        GetComponent<Animator>().SetTrigger(AllInteractions.INTERACTION_TRIGGER);
    }

    public override void MoveInteraction(bool asEcho)
    {
    }

    public override void EndInteraction(bool asEcho)
    {
    }
}
