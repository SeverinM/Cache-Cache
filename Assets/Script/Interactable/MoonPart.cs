using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonPart : Interactable
{
    [SerializeField]
    float draggingSensitivity = 0.2f;
    float baseSensitivity;

    enum Part
    {
        HIGH,
        LOW,
        NONE
    }

    static bool unlocked = false;
    static List<Part> takenParts = new List<Part>();
    [SerializeField]
    Part part;

    [SerializeField]
    int coeffDrag = 200;

    [SerializeField]
    float maxDistance = 0.5f;

    public float Ratio => transform.localPosition.magnitude / maxDistance;

    bool canMove = false;

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse,  Interactable echo = null)
    {
        if (btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON) && Progress == 0)
        {
            if (!echo)
            {
                if (!takenParts.Contains(part))
                {
                    canMove = true;
                    takenParts.Add(part);
                    baseSensitivity = mouse.sensitivity;
                    mouse.sensitivity = draggingSensitivity;
                    Debug.Log(gameObject);
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
            if (canMove && Progress == 0)
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

                if (temporaryPosition.magnitude / maxDistance <= 1)
                    transform.localPosition = temporaryPosition;
            }
        }
    }

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if ((btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON) || btn.Equals(MouseInputManager.MouseButton.NONE)) && !echo)
        {
            if (takenParts.Contains(part) && Progress == 0)
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

            mouse.sensitivity = baseSensitivity;
        }       
    }

    public override void OnNewValue()
    {
        if (!unlocked)
        {
            unlocked = true;
            foreach(TeleportSpot tp in GameObject.FindObjectsOfType<TeleportSpot>())
            {
                foreach(Draggable dragg in GameObject.FindObjectsOfType<Draggable>())
                {
                    dragg.AddSpot(tp);
                }
            }
        }
    }
}
