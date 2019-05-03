using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField]
    Camera cam1;

    [SerializeField]
    Camera cam2;

    private void Awake()
    {
        #if UNITY_EDITOR
            Rect rect = cam1.rect;
            rect.width = 0.5f;
            cam1.rect = rect;

            rect = cam2.rect;
            rect.x = 0.5f;
            rect.width = 0.5f;
            cam2.rect = rect;

            cam2.targetDisplay = 0;
        #endif

        if (Display.displays.Length > 1)
            Display.displays[1].Activate();
        if (Display.displays.Length > 2)
            Display.displays[2].Activate();
    }
}
