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

        if(!echo && !holdBy)
        {
            holdBy = this;
            currentSpeed += speed;
        }

        if (echo && holdBy)
        {
            GetComponent<Button>().interactable = false;
            otherButton.interactable = false;
            currentSpeed += speed;
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

        if (!echo && holdBy == this)
        {
            holdBy = null;
            currentSpeed = 0;
        }
        
        if (echo && !holdBy)
        {
            currentSpeed = 0;
            GetComponent<Button>().interactable = true;
            otherButton.interactable = true;
        }
    }

    private void Update()
    {
        source.RotateAround(target.transform.position, Vector3.up, currentSpeed * Time.deltaTime);
    }
}
