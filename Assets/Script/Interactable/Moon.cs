using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moon : Interactable
{
    [SerializeField]
    TeleportSpot tpSpot;

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        //Il ne doit rien y avoir dans notre spot mais avoir le spot occupé là bas
        if (!tpSpot.CurrentHold && tpSpot.GetOtherPart().CurrentHold)
        {
            Debug.Log("retransfert");
            tpSpot.GetOtherPart().StartCoroutine(tpSpot.GetOtherPart().Transfert());
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
