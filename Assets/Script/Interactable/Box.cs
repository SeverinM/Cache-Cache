using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Box : Interactable
{
    enum BoxPart
    {
        LOW, 
        HIGH
    }

    static bool done = false;

    [SerializeField]
    BoxPart part;

    [SerializeField]
    CameraWaypoints waypoints;

    [SerializeField]
    GameObject panelGame;

    Vector3 originPositionLocal;
    bool dragg = false;

    [SerializeField]
    float draggSpeed;

    [SerializeField]
    float maxDistance;

    Vector2 minNoStretch;
    Vector2 maxNoStretch;
    Vector2 minStretch;
    Vector2 maxStretch;

    [SerializeField]
    Image img;

    public float Ratio => Vector3.Distance(originPositionLocal, transform.localPosition) / maxDistance;

    protected override void Awake()
    {
        base.Awake();
        originPositionLocal = transform.localPosition;
        minNoStretch = img.GetComponent<RectTransform>().anchorMin;
        maxNoStretch = img.GetComponent<RectTransform>().anchorMax;
        if (part == BoxPart.HIGH)
        {
            minStretch = new Vector2(minNoStretch.x + 0.05f, minNoStretch.y);
            maxStretch = new Vector2(maxNoStretch.x - 0.05f, 1);
        }
        else
        {
            maxStretch = new Vector2(maxNoStretch.x - 0.05f, maxNoStretch.y);
            minStretch = new Vector2(minNoStretch.x + 0.05f, 0);
        }
    }

    private void Update()
    {
        if (img)
        {
            img.GetComponent<RectTransform>().anchorMin = Vector2.Lerp(minNoStretch, minStretch, Ratio);
            img.GetComponent<RectTransform>().anchorMax = Vector2.Lerp(maxNoStretch, maxStretch, Ratio);
        }
    }

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        dragg = true;
    }

    public override void MouseEnter(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
    }

    public override void MouseLeave(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
    }

    public override void MouseMove(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (!echo)
        {
            if (dragg)
            {
                Vector3 temporaryPosition = transform.localPosition + new Vector3(0, -mouse.delta.y * draggSpeed, 0);
                transform.localPosition = GetClampedPosition(temporaryPosition);
            }
        }
        else
        {
            transform.localPosition = echo.transform.localPosition;
        }
    }

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        dragg = false;
        transform.localPosition = originPositionLocal;
    }

    public Vector3 GetClampedPosition(Vector3 input)
    {
        Vector3 output = input;
        if (part == BoxPart.LOW)
        {
            return new Vector3(input.x, Mathf.Clamp(input.y , originPositionLocal.y - maxDistance, originPositionLocal.y) , input.z);
        }
        if (part == BoxPart.HIGH)
        {
            return new Vector3(input.x, Mathf.Clamp(input.y, originPositionLocal.y, originPositionLocal.y + maxDistance), input.z);
        }
        return output;
    }

    public override void OnNewValue()
    {
        base.OnNewValue();

        if (Progress == 0)
        {
            img.gameObject.SetActive(true);
        }
        //Done only once
        if (Progress == 1)
        {
            waypoints.StartNextWaypoint(NextStep);
            GetComponent<BoxCollider>().enabled = false;
            GetComponent<MeshRenderer>().enabled = false;
            img.gameObject.SetActive(false);
            panelGame.SetActive(true);
        }   
    }

    void NextStep()
    {
        if (!done)
        {
            done = true;
            foreach (Interactable inter in GameObject.FindObjectsOfType<Interactable>())
            {
                EnigmeManager.getInstance().StartCountdown();
                if (!(inter is Box))
                    inter.Progress++;
            }
        }
    }

    public override bool IsHandCursor()
    {
        return (Progress == 0);
    }
}
