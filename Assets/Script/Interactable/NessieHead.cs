using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NessieHead : Interactable
{
    bool reversed = false;

    [SerializeField]
    float duration;

    [SerializeField]
    AnimationCurve curve;

    [SerializeField]
    GameObject gobRoot;

    [SerializeField]
    Transform head;

    bool toggleState = false;
    public bool ToggleState
    {
        get
        {
            return toggleState;
        }
        set
        {
            toggleState = value;
        }
    }

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        //Anti echo
        if (!CanInteract) return;

        StartCoroutine(StartCoolDown());
        StartCoroutine(HeadUp());

        //Les deux parties ont toujours le meme etat
        toggleState = !toggleState;
    }

    IEnumerator StartCoolDown()
    {
        CanInteract = false;
        yield return new WaitForSeconds(duration);
        CanInteract = true;
    }

    public override void OnNewValue()
    {
        if (Progress == 1)
            CanInteract = true;
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
            head.localEulerAngles = new Vector3(value, 0, 0);
            yield return null;
        }

        reversed = !reversed;
        CanInteract = true;
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
