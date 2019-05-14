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
        //GameObject gob = new GameObject();
        //uint uDeviceIDSpeakers = AkSoundEngine.GetDeviceIDFromName(withName);

        //ulong[] listeners = new ulong[1];
        //listeners[0] = AkSoundEngine.GetAkGameObjectID(gob);
        //ulong NULL = 0;

        //string busStr = "System_J" + (allSoundOutput.Count + 1);
        //Debug.Log(busStr);
        //AkOutputSettings outputSettings = new AkOutputSettings(busStr, uDeviceIDSpeakers);
        //AkSoundEngine.AddOutput(outputSettings, out NULL, listeners, 1);
        //AkSoundEngine.RegisterGameObj(gob);
        //AkSoundEngine.SetListeners(gob, listeners, 1);


        //allSoundOutput.Add(gameObject);
    }

    public void PlaySound(int index , string nameEvent)
    {
        AkSoundEngine.PostEvent(nameEvent, allSoundOutput[index]);
    }
}
