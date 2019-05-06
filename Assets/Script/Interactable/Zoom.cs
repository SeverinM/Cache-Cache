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
    Vector3 destinationWorldLook;

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

        if (Application.isPlaying && actualFOV == minFOV && actualState.Equals(StateZoom.Waiting))
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
                    Ray ray = new Ray(mouse.cam.transform.position, mouse.lastCollisionPoint - mouse.cam.transform.position);

                    foreach(RaycastHit hit in Physics.RaycastAll(ray))
                    {
                        if (hit.collider.tag == Manager.MAQUETTE_TAG)
                        {
                            StartCoroutine(setFOWAndForward(StateZoom.Zooming));
                            destinationLook = hit.point;
                            break;
                        }
                    }
                }

                if (minFOV == actualFOV)
                {
                    originLook = destinationLook;
                    destinationLook = focusPoint.transform.position;
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
