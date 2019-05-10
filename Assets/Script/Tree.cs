using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Draggable
{
    public Squirrel squirrel;

    [SerializeField]
    float DelayDragging = 0.1f;

    float TimerDragging;

    TreeSpot actualSpot;
    TreeSpot temporaryTreeSpot;
    public TreeSpot TemporaryTreeSpot => temporaryTreeSpot;
    bool downDone = false;
    bool isDown = false;

    MouseInputManager.MousePointer currMouse;

    protected override void Awake()
    {
        base.Awake();
        actualSpot = GetComponentInChildren<TreeSpot>();
    }

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        TimerDragging = DelayDragging;
        SetTreeSpot(null);

        if (btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON))
            isDown = true;
    }

    public override void MouseEnter(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        base.MouseEnter(btn, mouse, echo);
    }

    public override void MouseLeave(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        base.MouseLeave(btn, mouse, echo);
    }

    private void Update()
    {
        if (TimerDragging > 0 && isDown)
        {
            TimerDragging -= Time.deltaTime;
            if (TimerDragging <= 0)
            {
                if (!downDone)
                {
                    downDone = true;
                    base.MouseDown(MouseInputManager.MouseButton.LEFT_BUTTON, currMouse, Echo);
                }
            }
        }
    }

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON))
        {
            isDown = false;
            if (downDone)
            {
                base.MouseUp(btn, mouse, echo);
                downDone = false;
            }
            else
            {
                if (!squirrel)
                    FouilleTree();
                else
                {
                    if (CompareTag("Hiver"))
                    {
                        squirrel.Progress++;
                    }
                    else
                    {
                        squirrel.NextJump();
                    }
                }
                    
            }            
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

    public void SetTreeSpot(TreeSpot spot)
    {
        if (actualSpot)
            actualSpot.transform.SetParent(null);

        actualSpot = spot;

        //actualSpot can be null
        if (actualSpot)
        {
            actualSpot.transform.SetParent(transform);
            transform.position = actualSpot.transform.position;
        }          
    }

    public override void ResetPosition()
    {
        if (!TemporaryTreeSpot)
            base.ResetPosition();
        else
        {
            SetTreeSpot(TemporaryTreeSpot);
            temporaryTreeSpot = null;
        }           
    }
}


