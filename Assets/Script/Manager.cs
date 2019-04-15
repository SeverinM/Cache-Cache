using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Manager : MonoBehaviour
{
    [SerializeField]
    GameObject referenceCanvas;

    [SerializeField]
    InputField input;

    [SerializeField]
    GameObject prefabIP;

    [SerializeField]
    CustomNetworkManager manager;

    Text txt;

    static Manager _instance;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(_instance.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    public static Manager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new GameObject().AddComponent<Manager>();
            DontDestroyOnLoad(_instance.gameObject);
        }
        return _instance;
    }

    public void StartAsHost()
    {
        Destroy(referenceCanvas);
        GameObject gob = Instantiate(prefabIP);
        string hostName = System.Net.Dns.GetHostName();
        System.Net.IPAddress[] allAddr = System.Net.Dns.GetHostEntry(hostName).AddressList;
        string addrIP = allAddr[allAddr.Length - 1].ToString();
        GameObject.FindObjectOfType<Text>().text = "Votre adresse est : " + addrIP;
        manager.StartHost();
        manager.CanvasServer = gob;
    }

    public void StartAsClient()
    {
        manager.networkAddress = input.text;
        manager.StartClient();
        Destroy(referenceCanvas);
    }
}
