using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : Interactable
{
    enum BoxPart
    {
        LOW, 
        HIGH
    }

    static bool done = false;

    [SerializeField]
    BoxPart part;
    Vector3 originPositionLocal;
    bool dragg = false;

    [SerializeField]
    float draggSpeed;

    [SerializeField]
    float maxDistance;

    public float Ratio => Vector3.Distance(originPositionLocal, transform.localPosition) / maxDistance;

    private void Awake()
    {
        originPositionLocal = transform.localPosition;
    }

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        dragg = true;
    }

    public override void MouseEnter(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
    }

    public override void MouseLeave(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
    }

    public override void MouseMove(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (!echo)
        {
            if (dragg)
            {
                Vector3 temporaryPosition = transform.localPosition + new Vector3(0, -mouse.delta.y * draggSpeed, 0);
                transform.localPosition = GetClampedPosition(temporaryPosition);
            }
        }
        else
        {
            transform.localPosition = echo.transform.localPosition;
        }
    }

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        dragg = false;
        transform.localPosition = originPositionLocal;
    }

    public Vector3 GetClampedPosition(Vector3 input)
    {
        Vector3 output = input;
        if (part == BoxPart.LOW)
        {
            return new Vector3(input.x, Mathf.Clamp(input.y , originPositionLocal.y - maxDistance, originPositionLocal.y) , input.z);
        }
        if (part == BoxPart.HIGH)
        {
            return new Vector3(input.x, Mathf.Clamp(input.y, originPositionLocal.y, originPositionLocal.y + maxDistance), input.z);
        }
        return output;
    }

    public override void OnNewValue()
    {
        base.OnNewValue();

        //Done only once
        if (Progress == 1)
        {
            if (!done)
            {
                done = true;
                foreach (Interactable inter in GameObject.FindObjectsOfType<Interactable>())
                {
                    if (!(inter is Box))
                        inter.Progress++;
                }
            }
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
        }       
    }
}
