using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Draggable
{
    [HideInInspector]
    public Squirrel squirrel;

    [SerializeField]
    float DelayDragging = 0.1f;

    float TimerDragging;
    bool dragged = false;

    private void Awake()
    {
        squirrel = null;
    }

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        TimerDragging = DelayDragging;
        base.MouseDown(btn, mouse, echo);
    }

    public override void MouseEnter(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        base.MouseEnter(btn, mouse, echo);
    }

    public override void MouseLeave(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        base.MouseLeave(btn, mouse, echo);
    }

    public override void MouseMove(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        TimerDragging -= Time.deltaTime;
        if (TimerDragging <= 0)
        {
            dragged = true;
            base.MouseMove(btn, mouse, echo);
        }
        
    }

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        

        if (squirrel && !dragged)
        {
            squirrel.NextJump();
        }
        else if (!dragged)
        {
            FouilleTree();
        }

        base.MouseUp(btn, mouse, echo);
        dragged = false;
    }

    public override void OnNewValue()
    {
        base.OnNewValue();
    }

    public void FouilleTree()
    {
        if(this.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("arbre_ete_ferme"))
        {
            this.GetComponent<Animator>().SetTrigger(Manager.TRIGGER_INTERACTION);
        }
    }
}


