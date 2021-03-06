﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    [SerializeField]
    Camera cam1;

    [SerializeField]
    EnigmeManager enn;

    [SerializeField]
    Camera cam2;

    [SerializeField]
    GameObject wwise;

    [SerializeField]
    GameObject plateau2;

    static Manager _instance;

    public const string TRIGGER_INTERACTION = "Interaction";
    public const string MAQUETTE_TAG = "Maquette";
    public const string TRIGGER_FOUND = "Found";
    public const string HOUSE_CLOSED = "maison_fermee";

    [SerializeField]
    float idleTimeBeforeReset = 180;
    float cursorTime;

    private void Awake()
    {
        cursorTime = idleTimeBeforeReset;
        if (!_instance)
            _instance = this;
        else
            Destroy(gameObject);


        if (Display.displays.Length > 1)
        {
            Display.displays[1].Activate();
        }
        AkSoundEngine.PostEvent("Play_amb_J1", gameObject);
        AkSoundEngine.PostEvent("Play_amb_J2", gameObject);
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
            Reset();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        //If idle for too long , reset the game
        cursorTime -= Time.deltaTime;

        if (cursorTime <= 0)
        {
            RefreshTimer();
            Reset();
        }
    }

    public void RefreshTimer()
    {
        cursorTime = idleTimeBeforeReset;
    }

    public void Reset()
    {
        AkSoundEngine.StopAll();
        Destroy(enn.gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PlaySound(MouseInputManager.MousePointer mouse, string name , bool otherPlayer = false)
    {
        //C'est le joueur 1
        if (mouse.cam == cam1)
        {
            AkSoundEngine.PostEvent(name + (otherPlayer ? "_J2" : "_J1"), gameObject);
        }

        //C'est le joueur 2
        else
        {
            AkSoundEngine.PostEvent(name + (otherPlayer ? "_J1" :  "_J2"), gameObject);
        }
    }

    public void PlayBoth(string name)
    {
        AkSoundEngine.PostEvent(name + "_J1", gameObject);
        AkSoundEngine.PostEvent(name + "_J2", gameObject);
    }

    public void PlayByDistance(string name , Transform trans , bool otherPlayer)
    {
        //C'est le joueur 1
        if (Vector3.Distance(trans.position , Vector3.zero) < 100)
        {
            AkSoundEngine.PostEvent(name + (otherPlayer ? "_J2" : "_J1"), gameObject);
        }

        //C'est le joueur 2
        else
        {
            AkSoundEngine.PostEvent(name + (otherPlayer ? "_J1" : "_J2"), gameObject);
        }
    }
}
