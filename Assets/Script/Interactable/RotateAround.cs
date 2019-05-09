﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (!btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON)) return;
        currentSpeed += speed;

        if (echo)
        {
            GetComponent<Button>().interactable = false;
            otherButton.interactable = false;
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
        if (btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON))
        {
            currentSpeed -= speed;
            if (echo)
            {
                GetComponent<Button>().interactable = true;
                otherButton.interactable = true;
            }
        }
    }

    private void Update()
    {
        source.RotateAround(target.transform.position, Vector3.up, currentSpeed * Time.deltaTime);
    }
}
