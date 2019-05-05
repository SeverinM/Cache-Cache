using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Zoom : Interactable
{
    [SerializeField]
    float timeZoom;

    [SerializeField]
    AnimationCurve curve;

    [SerializeField]
    Camera cam;

    [SerializeField]
    GameObject focusPoint;

    enum StateZoom
    {
        Zooming,
        Dezooming,
        Waiting
    }
    StateZoom actualState = StateZoom.Waiting;

    [SerializeField]
    float minFOV;
    float maxFOV;
    float actualFOV => cam.fieldOfView;
    Vector3 originLook;
    Vector3 destinationLook;

    // Start is called before the first frame update
    void Start()
    {
        maxFOV = actualFOV;
        if (focusPoint) cam.transform.LookAt(focusPoint.transform);
    }

    private void Update()
    {
        //Silent bugged error
        try
        {
            if (!Application.isPlaying && focusPoint)
                cam.transform.LookAt(focusPoint.transform);
        }
        catch (System.Exception exception){}
        
        if (Application.isPlaying && actualFOV < maxFOV)
        {
            cam.transform.LookAt(destinationLook);
        }
    }

    IEnumerator setFOWAndForward(StateZoom state)
    {
        actualState = state;
        float normalizedTime = 0;
        while (normalizedTime <= 1)
        {
            yield return null;
            normalizedTime += Time.deltaTime / timeZoom;
            cam.transform.LookAt(Vector3.Lerp(originLook, destinationLook, normalizedTime));
            cam.fieldOfView = Mathf.Lerp(minFOV, maxFOV, curve.Evaluate(state == StateZoom.Dezooming ? normalizedTime : 1 - normalizedTime));
        }
        actualState = StateZoom.Waiting;
    }

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (btn.Equals(MouseInputManager.MouseButton.RIGHT_BUTTON))
        {
            if (actualState.Equals(StateZoom.Waiting))
            {
                if (maxFOV == actualFOV)
                {
                    originLook = cam.transform.forward + cam.transform.position;
                    destinationLook =  mouse.lastCollisionPoint;
                    StartCoroutine(setFOWAndForward(StateZoom.Zooming));
                }

                if (minFOV == actualFOV)
                {
                    destinationLook = focusPoint.transform.position;
                    originLook = cam.transform.forward + cam.transform.position; 
                    StartCoroutine(setFOWAndForward(StateZoom.Dezooming));
                }
            }
            
        }
    }

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
    }

    public override void MouseMove(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
    }

    public override void MouseEnter(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
    }

    public override void MouseLeave(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
    }
}
