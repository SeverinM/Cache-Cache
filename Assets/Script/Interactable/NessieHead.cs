using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NessieHead : Interactable
{
    [SerializeField]
    Transform head;

    [SerializeField]
    AnimationCurve curve;

    [SerializeField]
    float duration;

    [SerializeField]
    Interactable dudeMouth;

    bool reversed = false;

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (Progress == 1)
            StartCoroutine(HeadUp());
    }

    public override void MouseEnter(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
    }

    public override void MouseLeave(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
    }

    public override void MouseMove(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
    }

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
    }

    public override void OnNewValue()
    {
        base.OnNewValue();
        if (dudeMouth)
            dudeMouth.Progress = Progress;
    }

    IEnumerator HeadUp()
    {
        CanInteract = false;
        float normalizedTime = 0;
        //reversed = fermé à ouvert
        while (normalizedTime <= 1)
        {
            normalizedTime += Time.deltaTime / duration;
            float value = Mathf.Lerp(0, -80 ,curve.Evaluate(reversed ? 1 - normalizedTime : normalizedTime));
            head.transform.localEulerAngles = new Vector3(value, 0, 0);
            yield return null;
        }

        if (dudeMouth)
            dudeMouth.CanInteract = !reversed;

        reversed = !reversed;
        CanInteract = true;
    }

}
