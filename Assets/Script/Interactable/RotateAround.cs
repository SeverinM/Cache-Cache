using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class RotateAround : Interactable
{
    [SerializeField]
    Transform source;

    [SerializeField]
    Transform target;

    [SerializeField]
    Button otherButton;

    [SerializeField]
    float speed;
    float currentSpeed = 0;
    public float CurrentSpeed => currentSpeed;

    static float GlobalSpeed => GameObject.FindObjectsOfType<RotateAround>().Sum(x => x.CurrentSpeed);
    bool isDown = false;

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (!btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON)) return;
       
        if (echo)
        {
            GetComponent<Button>().interactable = false;
            otherButton.interactable = false;
        }
        else
        {
            if (GlobalSpeed == 0)
            {
                currentSpeed += speed;
                isDown = true;
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
    }

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (!btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON)) return;
        
        if (echo)
        {
            GetComponent<Button>().interactable = true;
            otherButton.interactable = true;
        }
        else
        {
            if (CurrentSpeed != 0 && isDown)
            {
                currentSpeed = 0;
                isDown = false;
            }
        }
    }

    private void Update()
    {
        source.RotateAround(target.transform.position, Vector3.up, currentSpeed * Time.deltaTime);
    }
}
