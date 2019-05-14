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
        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
        }

        SoundManager.getInstance().AddSoundOutput("Realtek");
        SoundManager.getInstance().AddSoundOutput("NVIDIA");

        SoundManager.getInstance().PlaySound(0, "Play_Summer_Amb");
        SoundManager.getInstance().PlaySound(1, "Play_Winter_Amb");
    }
}
