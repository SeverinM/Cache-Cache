using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    [SerializeField]
    Camera cam1;

    [SerializeField]
    Camera cam2;

    [SerializeField]
    GameObject plateau2;

    public const string TRIGGER_INTERACTION = "Interaction";
    public const string MAQUETTE_TAG = "Maquette";

    private void Awake()
    {
        Debug.Log("kop");
        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
            cam2.transform.RotateAround(plateau2.transform.position, Vector3.up, 180);
        }       
    }
}
