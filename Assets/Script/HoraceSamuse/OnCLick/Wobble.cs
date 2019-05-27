using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wobble : Interactable
{
    public AnimationCurve sizeFluctuationWhenTouch = AnimationCurve.Constant(0, 1, 1);
    public float wobblingDuration = 1;

    private float LerpValue = 1;
    private float realScaleY;

    // Start is called before the first frame update
    void Start()
    {
        realScaleY = transform.localScale.y;
    }

    private void Update()
    {
        if(LerpValue != 1)
        {
            Vector3 tmp = this.transform.localScale;
            tmp.y = realScaleY * sizeFluctuationWhenTouch.Evaluate(LerpValue);
            this.transform.localScale = tmp;


            LerpValue += Time.deltaTime / wobblingDuration;
            if (LerpValue > 1)
                LerpValue = 1;
        }
    }

    [ContextMenu("Touch Me")]
    private void Touch()
    {
        LerpValue = 0;
    }


    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null){}
    public override void MouseEnter(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null){}
    public override void MouseLeave(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null){}
    public override void MouseMove(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null){}

    public override void MouseUp(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (btn.Equals(MouseInputManager.MouseButton.LEFT_BUTTON))
            Touch();
    }

}
