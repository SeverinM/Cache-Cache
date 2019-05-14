using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MoonPart : Interactable
{
    [SerializeField]
    float draggingSensitivity = 0.2f;
    float baseSensitivity;

    enum Part
    {
        HIGH,
        LOW,
        NONE
    }

    static bool unlocked = false;
    static List<TeleportSpot> tpSpots = new List<TeleportSpot>();
    static Dictionary<Part, GameObject> authorityTable = new Dictionary<Part, GameObject>();

    [SerializeField]
    TeleportSpot tpSpot;

    [SerializeField]
    Part part;

    [SerializeField]
    int coeffDrag = 200;

    [SerializeField]
    float maxDistance = 0.5f;

    public float Ratio => transform.localPosition.magnitude / maxDistance;

    protected bool canMove = false;

    protected override void Awake()
    {
        base.Awake();
        if (tpSpot) tpSpots.Add(tpSpot);
    }

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse,  Interactable echo = null)
    {
        if (btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON) && Progress == 0)
        {
            if (!echo)
            {
                if (hasControl(part))
                {
                    canMove = true;
                    authorityTable[part] = gameObject;
                    baseSensitivity = mouse.sensitivity;
                    mouse.sensitivity = draggingSensitivity;
                }
            }
        }
    }

    public override void MouseEnter(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
    }

    public override void MouseLeave(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
    }

    public override void MouseMove(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse,  Interactable echo = null)
    {       
        if (echo && Progress == 0)
        {
            transform.localPosition = Echo.transform.localPosition;
        }
        else
        {
            if (canMove && Progress == 0)
            {
                Vector3 temporaryPosition = transform.localPosition + new Vector3(0, -mouse.delta.y / coeffDrag, 0);
                if (part.Equals(Part.HIGH) && temporaryPosition.y < 0)
                {
                    temporaryPosition = Vector3.zero;
                }
                if (part.Equals(Part.LOW) && temporaryPosition.y > 0)
                {
                    temporaryPosition = Vector3.zero;
                }

                if (temporaryPosition.magnitude / maxDistance <= 1)
                    transform.localPosition = temporaryPosition;
            }
        }
    }

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON)  && !echo)
        {
            if (hasControl(part) && Progress == 0)
            {
                authorityTable.Remove(part);

                //Moon not locked yet
                if (progress == 0)
                {
                    transform.localPosition = Vector3.zero;
                    if (Echo)
                    {
                        Echo.transform.localPosition = Vector3.zero;
                    }
                }

                mouse.sensitivity = baseSensitivity;
                canMove = false;
            }

        }       
    }

    public override void OnNewValue()
    {
        base.OnNewValue();
        if (!unlocked && Progress == 1)
        {
            unlocked = true;
            foreach(TeleportSpot tp in tpSpots.Distinct())
            {
                tp.Init();
                foreach(Draggable dragg in GameObject.FindObjectsOfType<Draggable>())
                {
                    dragg.AddSpot(tp);
                }
            }
        }
    }

    bool hasControl(Part which)
    {
        return (!authorityTable.ContainsKey(which) || authorityTable[which] == gameObject);
    }
}
