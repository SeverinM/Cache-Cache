using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AK;

public class SoundManager : MonoBehaviour
{
    static SoundManager _instance;

    List<GameObject> allSoundOutput = new List<GameObject>();

    private void Awake()
    {
        if (_instance)
            Destroy(gameObject);
        else
            _instance = this;
    }

    public static SoundManager getInstance()
    {
        if (!_instance)
        {
            _instance = new GameObject().AddComponent<SoundManager>();
        }
        return _instance;
    }

    public void AddSoundOutput(string withName)
    {
        uint id = AkSoundEngine.GetDeviceIDFromName(withName);
        AkOutputSettings outputSettings = new AkOutputSettings("Output" + allSoundOutput.Count, id);
        uint nb;

        ulong[] listeners = new ulong[1];
        GameObject gob = new GameObject();
        listeners[0] = AkSoundEngine.GetAkGameObjectID(gob);
        ulong NULL = 0;

        AkSoundEngine.AddOutput(outputSettings, out NULL, listeners, 1);

        GameObject audioSource = new GameObject();
        AkSoundEngine.RegisterGameObj(audioSource);
        AkSoundEngine.SetListeners(gob, listeners, 1);
        allSoundOutput.Add(audioSource);
    }

    public void PlaySound(int index , string nameEvent)
    {
        if (index < allSoundOutput.Count)
        {
            AkSoundEngine.PostEvent(nameEvent, allSoundOutput[index]);
        }
    }
}
