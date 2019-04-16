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
    GameObject prefabIP;

    [SerializeField]
    CustomNetworkManager manager;

    Text txt;
    bool found = false;

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
        manager.StartHost();
    }

    public void StartAsClient(string addr)
    {
        manager.networkAddress = addr;
        manager.StartClient();
        Destroy(referenceCanvas);
    }

    public void Discovered(System.Net.IPEndPoint ip , string data)
    {
        found = true;
        Debug.Log(ip.ToString());
        StartAsClient(ip.ToString());
    }

    IEnumerator SearchingCoroutine()
    {
        Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorDiscovery.InitializeFinder();
        while (!found)
        {
            yield return new WaitForSeconds(0.5f);
            Mirror.LiteNetLib4Mirror.LiteNetLib4MirrorDiscovery.SendDiscoveryRequest("START");
        }
    }
}
