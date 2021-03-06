﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : Draggable
{
    public Squirrel squirrel;

    [SerializeField]
    float DelayDragging = 0.1f;
    float TimerDragging;

    bool downDone = false;
    bool isDown = false;

    MouseInputManager.MousePointer currMouse;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        TimerDragging = DelayDragging;

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
                Manager.GetInstance().PlayByDistance("Play_tree_move", transform, false);
                base.MouseUp(btn, mouse, echo);
                downDone = false;
            }
            else
            {
                if (CurrentSpot is TeleportSpot) return;
                if (!squirrel)
                {
                    FouilleTree();
                }                  
                else
                {
                    if (gameObject.tag == "Hiver")
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
        if (tag == "Hiver")
        {
            Manager.GetInstance().PlayByDistance("Play_tree_twist", transform, false);
        }

        if (tag == "Ete")
        {
            Manager.GetInstance().PlayByDistance("Play_tree_open", transform, false);
            StartCoroutine(PlayClose());
        }

        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("arbre_ete_ferme") || GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("arbre_hiver_repos"))
            GetComponent<Animator>().SetTrigger(Manager.TRIGGER_INTERACTION);
    }

    IEnumerator PlayClose()
    {
        yield return new WaitForSeconds(1);
        Manager.GetInstance().PlayByDistance("Play_tree_close", transform, false);
    }

    public override bool IsHandCursor()
    {
        return CanInteract;
    }
}


