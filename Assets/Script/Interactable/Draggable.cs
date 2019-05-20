using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Draggable : Interactable
{
    [SerializeField]
    List<Spot> allSpot;

    [SerializeField]
    Vector3 relativeOffsetMoon;

    [SerializeField]
    protected Spot currentSpot;
    public Spot CurrentSpot
    {
        get
        {
            return currentSpot;
        }
        set
        {
            currentSpot = value;
        }
    }
    

    protected bool dragging = false;
    public bool Dragging => dragging;

    protected GameObject lastTouchedGameObject;
    protected Vector3 origin;

    protected override void Awake()
    {
        base.Awake();
        foreach(Spot sp in allSpot)
        {
            sp.SetValue(this, false);
        }
    }

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON))
        {
            if (currentSpot)
            {
                currentSpot.PressSpot(this);
            }

            origin = transform.position;
            dragging = true;
            foreach (Spot sp in allSpot)
            {
                sp.SetValue(this, true);
            }
        }
    }

    public override void MouseEnter(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
    }

    public override void MouseLeave(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
    }

    public override void MouseMove(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (dragging)
        {
            Vector3 position = mouse.obj.transform.position;
            Ray ray = mouse.cam.ScreenPointToRay(new Vector3(position.x, position.y, 0.01f));

            foreach (RaycastHit hit in Physics.RaycastAll(ray).OrderBy(x => Vector3.Distance(ray.origin, x.point)))
            {
                //Evite les raycast avec lui meme
                if (hit.collider.gameObject == gameObject) continue;

                //Ce qu'on survole est valide ?
                if (hit.collider.GetComponent<Spot>())
                {
                    if (hit.collider.GetComponent<Spot>().CurrentHold)
                    {
                        transform.position = hit.point;
                        break;
                    }

                    if (lastTouchedGameObject != hit.collider.gameObject)
                    {
                        //On ne survole plus le meme spot
                        if (lastTouchedGameObject && lastTouchedGameObject.GetComponent<Spot>())
                        {
                            lastTouchedGameObject.GetComponent<Spot>().ExitSpot(this);
                        }
                        if (hit.collider.GetComponent<Spot>() && !hit.collider.GetComponent<Spot>().CurrentHold)
                        {
                            hit.collider.GetComponent<Spot>().EnterSpot(this);
                        }
                    }
                    lastTouchedGameObject = hit.collider.gameObject;
                    break;
                }
                
                else
                {
                    transform.position = hit.point;
                    lastTouchedGameObject = hit.collider.gameObject;
                    return;
                }
                
            }
        }
    }

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON))
        {
            dragging = false;
            if (lastTouchedGameObject && lastTouchedGameObject.GetComponent<Spot>() && lastTouchedGameObject.GetComponent<Spot>().CurrentHold == null)
            {
                //Appell� uniquement quand le spot est different
                if (CurrentSpot != null && CurrentSpot != lastTouchedGameObject.GetComponent<Spot>())
                {
                    CurrentSpot.HoldObjectLeft(this);
                }

                CurrentSpot = lastTouchedGameObject.GetComponent<Spot>();
                lastTouchedGameObject.GetComponent<Spot>().ReleaseSpot(this);
            }
            else
            {
                if (currentSpot)
                {
                    currentSpot.ResetSpot(this);
                }
                else
                    transform.position = origin;
            }

            foreach(Spot sp in allSpot)
            {
                sp.SetValue(this, false);
            }
        }
    }

    public void AddSpot(Spot sp)
    {
        allSpot.Add(sp);
    }

    public void RemoveSpot(Spot sp)
    {
        allSpot.Remove(sp);
    }

    public virtual void ResetPosition()
    {
        transform.position = origin;
    }
}
