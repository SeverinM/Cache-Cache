using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonPart : Interactable
{
    enum Part
    {
        HIGH,
        LOW,
        NONE
    }

    static List<Part> takenParts = new List<Part>();
    [SerializeField]
    Part part;

    [SerializeField]
    int coeffDrag = 200;

    bool canMove = false;

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse,  Interactable echo = null)
    {
        if (btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON))
        {
            if (!echo)
            {
                if (!takenParts.Contains(part))
                {
                    canMove = true;
                    takenParts.Add(part);
                }
            }
        }
    }

    public override void MouseEnter(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
    }

    public override void MouseLeave(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
    }

    public override void MouseMove(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse,  Interactable echo = null)
    {       
        if (echo)
        {
            transform.localPosition = Echo.transform.localPosition;
        }
        else
        {
            if (canMove)
            {
                Vector3 temporaryPosition = transform.localPosition + new Vector3(0, -mouse.delta.y / coeffDrag, 0);
                if (part.Equals(Part.HIGH) && temporaryPosition.y < 0)
                {
                    temporaryPosition = Vector3.zero;
                }
                if (part.Equals(Part.LOW) && temporaryPosition.y > 0)
                {
                    temporaryPosition = Vector3.zero;
                }
                transform.localPosition = temporaryPosition;
            }
        }
    }

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if ((btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON) || btn.Equals(MouseInputManager.MouseButton.NONE)) && !echo)
        {
            if (takenParts.Contains(part))
            {
                takenParts.Remove(part);

                //Moon not locked yet
                if (progress == 0)
                {
                    transform.localPosition = Vector3.zero;
                    if (Echo)
                    {
                        Echo.transform.localPosition = Vector3.zero;
                    }
                }

                canMove = false;
            }
        }
    }
}
