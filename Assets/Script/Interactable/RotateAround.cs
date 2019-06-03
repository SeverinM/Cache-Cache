using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class RotateAround : Interactable
{
    [SerializeField]
    Transform source;

    [SerializeField]
    Transform target;

    [SerializeField]
    Button otherButton;
    public Button OtherButton => otherButton;

    [SerializeField]
    float speed;
    float currentSpeed = 0;

    public float CurrentSpeed
    {
        get
        {
            return currentSpeed;
        }

        set
        {
            currentSpeed = value;
        }
    }
    static RotateAround holdBy = null;

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (!btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON)) return;
        if (echo) return;

        Debug.Log("down");
        if (!holdBy)
        {
            holdBy = this;
            currentSpeed += speed;
            (Echo as RotateAround).currentSpeed += speed;
            Echo.GetComponent<Button>().interactable = false;
            (Echo as RotateAround).OtherButton.interactable = false;
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
    }

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (!btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON)) return;
        if (echo) return;
        Debug.Log("up");
        Debug.Log("=======");

        if (holdBy == this)
        {
            holdBy = null;
            currentSpeed = 0;
            (Echo as RotateAround).currentSpeed = 0;
            Echo.GetComponent<Button>().interactable = true;
            (Echo as RotateAround).OtherButton.interactable = true;
        }
    }

    private void Update()
    {
        source.RotateAround(target.transform.position, Vector3.up, currentSpeed * Time.deltaTime);
    }
}
