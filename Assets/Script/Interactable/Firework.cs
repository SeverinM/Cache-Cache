﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firework : Draggable
{
    [SerializeField]
    float duration;

    public override void MouseDown(MouseInputManager.MouseButton btn, MouseInputManager.MousePointer mouse, Interactable echo = null)
    {
        if (Progress == 0)
            base.MouseDown(btn, mouse, echo);
    }

    public override void OnNewValue()
    {
        base.OnNewValue();
        if (Progress == 1)
            Debug.Log("I call this an absolute win");
    }

    IEnumerator LaunchFirework()
    {
        float normalizedTime = 0;
        while (normalizedTime <= 1)
        {
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
    }
}
