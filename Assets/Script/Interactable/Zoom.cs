using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : Interactable
{
    [SerializeField]
    float timeZoom;

    [SerializeField]
    AnimationCurve curve;

    [SerializeField]
    Camera cam;

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
    Vector3 originForward;
    Vector3 destinationForward;

    // Start is called before the first frame update
    void Start()
    {
        maxFOV = actualFOV;
    }

    IEnumerator setFOWAndForward(StateZoom state)
    {
        actualState = state;
        float normalizedTime = 0;
        while (normalizedTime <= 1)
        {
            yield return null;
            normalizedTime += Time.deltaTime / timeZoom;
            cam.transform.forward = Vector3.Lerp(originForward, destinationForward, curve.Evaluate(normalizedTime));
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
                    originForward = cam.transform.forward;
                    destinationForward = cam.transform.worldToLocalMatrix.MultiplyPoint(mouse.lastCollisionPoint);
                    StartCoroutine(setFOWAndForward(StateZoom.Zooming));
                }

                if (minFOV == actualFOV)
                {
                    destinationForward = originForward;
                    originForward = cam.transform.forward;
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
