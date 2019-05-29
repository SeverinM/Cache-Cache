using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spline;

public class Firework : Draggable
{
    [SerializeField]
    float duration;

    [SerializeField]
    BezierSpline spline;

    [SerializeField]
    Animator anim;

    [SerializeField]
    Animator anim2;

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (Progress == 0)
            base.MouseDown(btn, mouse, echo);
    }

    public override void OnNewValue()
    {
        if (Progress == 1)
            StartCoroutine(LaunchFirework());
    }

    public override bool IsHandCursor()
    {
        return CanInteract;
    }

    IEnumerator LaunchFirework()
    {
        yield return new WaitForSeconds(0.2f);
        AkSoundEngine.PostEvent("Play_fireworks_launch", gameObject);
        GetComponent<ParticleSystem>().Play();

        float normalizedTime = 0;
        while (normalizedTime <= 1)
        {
            normalizedTime += Time.deltaTime / duration;
            transform.position = spline.GetPoint(normalizedTime);
            transform.up = spline.GetDirection(normalizedTime);
            yield return null;
        }

        anim.SetTrigger(Manager.TRIGGER_INTERACTION);
        anim2.SetTrigger(Manager.TRIGGER_INTERACTION);
        GetComponent<ParticleSystem>().Stop();
        AkSoundEngine.PostEvent("Play_fireworks_explode", gameObject);
        Destroy(gameObject);
    }
}
