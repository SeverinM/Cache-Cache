﻿using System.Collections;
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
    Animator animGlace;

    [SerializeField]
    ParticleSystem boom;

    [Range(0,1)]
    [SerializeField]
    float progressTrigger;

    [SerializeField]
    float timeBeforeNessies;

    bool done = false;

    [SerializeField]
    Animator anim;

    [SerializeField]
    Animator anim2;

    MouseInputManager.MousePointer lastTouched;

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        lastTouched = mouse;
        if (Progress == 0)
            base.MouseDown(btn, mouse, echo);
    }

    public override void OnNewValue()
    {
        if (Progress == 1)
        {
            transform.parent = null;
            spline.transform.parent = null;
            StartCoroutine(LaunchFirework());
        }
            
    }

    public override bool IsHandCursor()
    {
        return CanInteract;
    }

    IEnumerator LaunchFirework()
    {
        yield return new WaitForSeconds(0.2f);
        AkSoundEngine.PostEvent("Play_rocket_launch", gameObject);
        GetComponent<ParticleSystem>().Play();

        float normalizedTime = 0;
        while (normalizedTime <= 1)
        {
            normalizedTime += Time.deltaTime / duration;
            transform.position = spline.GetPoint(normalizedTime);
            transform.up = spline.GetDirection(normalizedTime);
            if (normalizedTime >= progress && !done)
            {
                done = true;
                animGlace.SetTrigger(Manager.TRIGGER_INTERACTION);
                boom.Play();
            }

            yield return null;
        }

        StartCoroutine(NessieCome());
        GetComponent<ParticleSystem>().Stop();
        AkSoundEngine.PostEvent("Play_rocket_explode", gameObject);
    }

    IEnumerator NessieCome()
    {
        yield return new WaitForSeconds(timeBeforeNessies);
        AkSoundEngine.PostEvent("Play_Nessie_out", gameObject);
        anim.SetTrigger(Manager.TRIGGER_INTERACTION);
        anim2.SetTrigger(Manager.TRIGGER_INTERACTION);
        EnigmeManager.nessie_out = true;
        Destroy(gameObject);
    }
}
