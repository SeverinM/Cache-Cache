﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{

    bool canInteract = true;
    public bool CanInteract => canInteract;

    [SerializeField]
    protected Interactable echo;
    public Interactable Echo => echo;

    [SerializeField]
    protected bool block;
    public bool Block => block;

    //Incrementé quand une enigme est resolu
    protected int progress = 0;
    public int Progress
    {
        get
        {
            return progress;
        }
        set
        {
            progress = value;
            OnNewValue();
        }
    }


    public abstract void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null);
    public abstract void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null);
    public abstract void MouseMove(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse ,Interactable echo = null);
    public abstract void MouseEnter(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null);
    public abstract void MouseLeave(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null);
    public virtual void OnNewValue() { }
}