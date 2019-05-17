using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : Interactable
{
    [SerializeField]
    GameObject panelStart;

    [SerializeField]
    Interactable boxUp;

    [SerializeField]
    Interactable boxDown;

    private void Awake()
    {
        canInteract = true;
    }

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
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
        if (btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON) && Progress == -1)
        {
            panelStart.SetActive(false);
            boxDown.Progress++;
            boxUp.Progress++;
        }
    }
}
