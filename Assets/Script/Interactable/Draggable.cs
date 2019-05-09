using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Draggable : Interactable
{
    [SerializeField]
    List<Spot> allSpot;

    protected bool dragging = false;
    public bool Dragging => dragging;

    protected GameObject lastTouchedGameObject;
    protected Vector3 origin;

    private void Awake()
    {
        foreach(Spot sp in allSpot)
        {
            sp.SetValue(this, false);
        }
    }

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON) && Progress >= 0)
        {
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
                if (hit.collider.CompareTag(Manager.MAQUETTE_TAG) || hit.collider.GetComponent<Spot>())
                {
                    transform.position = hit.point;
                    lastTouchedGameObject = hit.collider.gameObject;
                    break;
                }
            }
        }
    }

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON) && Progress >= 0)
        {
            dragging = false;
            if (lastTouchedGameObject && lastTouchedGameObject.GetComponent<Spot>())
            {
                lastTouchedGameObject.GetComponent<Spot>().ReleaseSpot(this);
            }
            else
            {
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
}
