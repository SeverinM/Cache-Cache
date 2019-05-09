using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Draggable
{
    public Squirrel squirrel;

    [SerializeField]
    float DelayDragging = 0.1f;

    float TimerDragging;
    bool dragged = false;

    public Transform ActualSpot;

    private void Awake()
    {
        squirrel = null;
        ActualSpot.GetComponentInChildren<Collider>().enabled = false;
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
            ActualSpot.SetParent(null);
            base.MouseMove(btn, mouse, echo);
        }
        
    }

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if(btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON))
        {
            if (!dragged)
            {
                if (!squirrel)
                    FouilleTree();

                else
                    squirrel.NextJump();
            }

            if(lastTouchedGameObject && lastTouchedGameObject.tag == "TreeSpot")
            {
                ActualSpot.GetComponentInChildren<Collider>().enabled = true;
                
                base.MouseUp(btn, mouse, echo);
                ActualSpot = lastTouchedGameObject.transform;
                ActualSpot.GetComponentInChildren<Collider>().enabled = false;
            }
            else
            {
                transform.position = origin;
            }

            dragged = false;
        }
    }

    public override void OnNewValue()
    {
        base.OnNewValue();
    }

    public void FouilleTree()
    {
        this.GetComponent<Animator>().SetTrigger(Manager.TRIGGER_INTERACTION);
    }
}


