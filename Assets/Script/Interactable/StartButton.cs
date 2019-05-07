using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : Interactable
{
    [SerializeField]
    GameObject panelStart;

    [SerializeField]
    GameObject panelGame;

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
        if (btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON))
        {
            panelStart.SetActive(false);
            panelGame.SetActive(true);
            if (!echo)
            {
                foreach(Interactable inter in GameObject.FindObjectsOfType<Interactable>())
                {
                    inter.Progress++;
                }
            }
        }
    }
}
