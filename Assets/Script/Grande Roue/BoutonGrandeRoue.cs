﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoutonGrandeRoue : Interactable
{
    [HideInInspector]
    public bool turnTheWheel = false;

    [SerializeField]
    Animator anim;

    bool isOnButton = false;
    bool isPressingButton = false;

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        Debug.Log("down");
        isPressingButton = true;
        anim.SetBool("appui", true);
    }

    public override void MouseEnter(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        isOnButton = true;
    }

    public override void MouseLeave(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        isOnButton = false;
    }

    public override void MouseMove(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        
    }

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        Debug.Log("up");
        anim.SetBool("appui", false);
        isPressingButton = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isOnButton && isPressingButton)
        {
            turnTheWheel = true;
        }
        else
        {
            turnTheWheel = false;
        }
    }
}