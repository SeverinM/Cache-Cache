using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [SerializeField]
    bool block;

    [SerializeField]
    protected Interactable echo;
    public Interactable Echo => echo;
    public bool Block => block;

    public abstract void MouseDown(MouseInputManager.MouseButton btn,  Interactable echo = null);
    public abstract void MouseUp(MouseInputManager.MouseButton btn, Interactable echo = null);
    public abstract void MouseMove(MouseInputManager.MouseButton btn, Vector2 delta, Interactable echo = null);
    public abstract void MouseEnter(MouseInputManager.MouseButton btn, Interactable echo = null);
    public abstract void MouseLeave(MouseInputManager.MouseButton btn, Interactable echo = null);
}
