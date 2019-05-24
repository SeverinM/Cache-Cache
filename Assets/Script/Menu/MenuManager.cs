using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    static MenuManager _instance;

    //On commence toujours a l'index 0
    int currentIndex = 0;
    public int CurrentIndex
    {
        get
        {
            return currentIndex;
        }
        set
        {
            NewValue(currentIndex, value);
            currentIndex = value;
        }
    }

    private void Awake()
    {
        if (!_instance)
            _instance = this;
        else
            Destroy(gameObject);
    }

    public static MenuManager GetInstance()
    {
        if (!_instance)
        {
            _instance = new GameObject().AddComponent<MenuManager>();
        }
        return _instance;
    }

    void NewValue(int oldValue, int newValue)
    {

    }
}
