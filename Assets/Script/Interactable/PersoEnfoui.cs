using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersoEnfoui : Interactable
{
    [SerializeField]
    float maxDistance;

    [SerializeField]
    float sensi;

    [SerializeField]
    List<Transform> potentialMoonLandings;

    [SerializeField]
    float duration;

    Vector3 origin;

    public float Ratio => Vector3.Distance(transform.position, origin) / maxDistance;

    bool dragging;

    protected override void Awake()
    {
        origin = transform.position;
    }

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        dragging = true;
        origin = transform.position;
    }

    public override void MouseEnter(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
    }

    public override void MouseLeave(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
    }

    public override void MouseMove(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (Progress == 0)
        {
            Vector3 temporaryPosition = transform.position + new Vector3(0, -mouse.delta.y * sensi, 0);
            temporaryPosition = new Vector3(temporaryPosition.x, Mathf.Clamp(temporaryPosition.y, origin.y, origin.y + maxDistance), temporaryPosition.z);
            transform.position = temporaryPosition;
        }
    }

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (Progress == 0)
        {
            dragging = false;
            transform.position = origin;
        }
    }

    public override void OnNewValue()
    {
        base.OnNewValue();
        if (Progress == 1)
        {
            EnigmeManager.getInstance().DiscoveredCharacter(potentialMoonLandings, transform, Manager.TRIGGER_INTERACTION,  duration);
        }
    }

    public override bool IsHandCursor()
    {
        return true;
    }
}
