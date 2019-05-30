using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    [SerializeField]
    Camera cam1;

    [SerializeField]
    GameObject wwiseGlobal;

    [SerializeField]
    Camera cam2;

    [SerializeField]
    GameObject plateau2;

    static Manager _instance;

    public const string TRIGGER_INTERACTION = "Interaction";
    public const string MAQUETTE_TAG = "Maquette";
    public const string TRIGGER_FOUND = "Found";
    public const string HOUSE_CLOSED = "maison_fermee";

    private void Awake()
    {
        if (!_instance)
            _instance = this;
        else
            Destroy(gameObject);


        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
        }

        //SoundManager.getInstance().AddSoundOutput("Realtek");
        //SoundManager.getInstance().AddSoundOutput("NVIDIA");

        //SoundManager.getInstance().PlaySound(0, "Play_Summer_Amb");
        //SoundManager.getInstance().PlaySound(0, "Play_Winter_Amb");
        AkSoundEngine.PostEvent("Play_Amb_Playtest", gameObject);
    }

    public static Manager GetInstance()
    {
        if (!_instance)
            _instance = new GameObject().AddComponent<Manager>();

        return _instance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Destroy(wwiseGlobal);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
