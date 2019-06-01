using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NessieTail : Interactable
{
    bool busy = false;

    [SerializeField]
    AnimationCurve curve;

    [SerializeField]
    float duration;

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (!busy)
            StartCoroutine(TailRotate());
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

    IEnumerator TailRotate()
    {
        busy = true;
        float normalizedTime = 0;
        while (normalizedTime < 1)
        {
            normalizedTime += Time.deltaTime / duration;
            transform.localEulerAngles = new Vector3(0, Mathf.Lerp(0, 720, curve.Evaluate(normalizedTime)));
            yield return null;
        }
        busy = false;
    }
}
