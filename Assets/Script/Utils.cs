using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public static void DisableAllOthersCam(Camera cam)
    {
        foreach(Camera camera in GameObject.FindObjectsOfType<Camera>())
        {
            if (cam != camera)
            {
                camera.enabled = false;
            }
        }
        cam.enabled = true;
    }
}
