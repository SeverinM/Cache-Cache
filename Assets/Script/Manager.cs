using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Net.NetworkInformation;

public class Manager : MonoBehaviour
{
    [SerializeField]
    GameObject referenceCanvas;

    [SerializeField]
    GameObject prefabIP;

    [SerializeField]
    int port;

    [SerializeField]
    CustomNetworkManager manager;

    const string REQUEST_DISCOVERY = "CacheCache";
    const string DISCOVERY_FOUND = "CacheCacheOK";

    Text txt;
    bool found = false;

    UdpClient clientUdp;
    IPEndPoint interNetwork;

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

        //Ecoute pour discovery
        clientUdp = new UdpClient(port);

        byte[] Response = Encoding.ASCII.GetBytes(DISCOVERY_FOUND);
        //On ecoute toutes les interfaces reseaux du serveur
        interNetwork = new IPEndPoint(IPAddress.Any, 0);
        clientUdp.BeginReceive(new AsyncCallback(ServerResponse), null);
    }

    void ServerResponse(IAsyncResult ar)
    {
        //Quand le serveur recoit quelque chose , renvoit autre chose
        clientUdp.EndReceive(ar, ref interNetwork);
        byte[] Response = Encoding.ASCII.GetBytes(GetLocalIPAddress());
        Debug.Log(GetLocalIPAddress());

        IPAddress addr = IPAddress.Parse(interNetwork.Address.ToString());

        clientUdp.Send(Response, Response.Length, new IPEndPoint(addr, port));
        Debug.Log("le serveur a envoyé " + DISCOVERY_FOUND + " a " + addr.ToString());
    }

    public void StartResearch()
    {
        if (clientUdp == null)
            StartCoroutine(Research());
    }

    IEnumerator Research()
    {
        clientUdp = new UdpClient(port);
        byte[] RequestData = Encoding.ASCII.GetBytes(REQUEST_DISCOVERY);
        interNetwork = new IPEndPoint(IPAddress.Any, 0);
        clientUdp.EnableBroadcast = true;
        clientUdp.BeginReceive(new AsyncCallback(ClientResponse), null);
        while (!found)
        {
            IPAddress addr = IPAddress.Parse("10.1.61.255");
            Debug.Log("recherche : " + Time.timeSinceLevelLoad);
            clientUdp.Send(RequestData, RequestData.Length, new IPEndPoint(addr, port));
            yield return new WaitForSeconds(1f);
        }
    }

    public void StartAsClient(string addr)
    {
        manager.networkAddress = addr;
        manager.StartClient();
        Destroy(referenceCanvas);       
    }

    void ClientResponse(IAsyncResult ar)
    {
        Debug.Log("reponse recu client");
        if (!found)
        {
            string addr = Encoding.ASCII.GetString(clientUdp.EndReceive(ar, ref interNetwork));
            Debug.Log(addr);
            if (addr != "0.0.0.0")
            {
                Debug.Log(addr);
                found = true;
                StartAsClient(addr);
            }
        }      
    }

    public static string GetLocalIPAddress()
    {
        string hostName = Dns.GetHostName();
        IPAddress[] allAddr = Dns.GetHostEntry(hostName).AddressList;
        return allAddr[allAddr.Length - 1].ToString();
    }
}
