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
using System.Linq;

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

    string connectIp = "";
    bool connecting = false;

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

    private void Update()
    {
        if (connecting)
        {
            connecting = false;
            Connect(connectIp);
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
        byte[] Response = Encoding.ASCII.GetBytes(DISCOVERY_FOUND);
        IPAddress addr = IPAddress.Parse(interNetwork.Address.ToString());

        clientUdp.Send(Response, Response.Length, new IPEndPoint(addr, port));
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
        while (!found)
        {
            clientUdp.BeginReceive(new AsyncCallback(ClientResponse), null);
            foreach(string ip in GetBroadcastAdress().Distinct())
            {
                IPAddress addr = IPAddress.Parse(ip);
                clientUdp.Send(RequestData, RequestData.Length, new IPEndPoint(addr, port));
                Debug.Log(ip);
                yield return new WaitForSeconds(0.1f);
            }            
            yield return new WaitForSeconds(1f);
        }
    }

    public void Connect(string ip)
    {
        Destroy(referenceCanvas);
        manager.networkAddress = ip;
        manager.StartClient();
    }

    void ClientResponse(IAsyncResult ar)
    {
        if (!found)
        {
            string Response = Encoding.ASCII.GetString(clientUdp.EndReceive(ar, ref interNetwork));
            string addr = interNetwork.Address.ToString();
            if (Response == DISCOVERY_FOUND)
            {
                string addrCopy = (string)addr.Clone();
                found = true;
                connectIp = addrCopy;
                connecting = true;
            }
        }
    }

    public static List<string> GetBroadcastAdress()
    {
        List<string> output = new List<string>();
        string hostName = Dns.GetHostName();
        IPAddress[] allAddr = Dns.GetHostEntry(hostName).AddressList;
        IPAddress mask = null;
        bool stop = false;

        foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (stop) break;
            foreach (UnicastIPAddressInformation uipi in ni.GetIPProperties().UnicastAddresses)
            {
                string tempAddr = uipi.Address.ToString();
                //Pas d'adresse IPV4
                IPAddress addr = uipi.Address;
                if (!addr.ToString().Contains(":") && !addr.ToString().StartsWith("0."))
                {
                    mask = uipi.IPv4Mask;
                    output.Add(SplitBytes(addr, mask).ToString());
                }                   
            }
        }

        return output;
    }

    public static IPAddress SplitBytes(IPAddress address, IPAddress subnetMask)
    {
        byte[] ipAdressBytes = address.GetAddressBytes();
        byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

        if (ipAdressBytes.Length != subnetMaskBytes.Length)
            throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

        byte[] broadcastAddress = new byte[ipAdressBytes.Length];
        for (int i = 0; i < broadcastAddress.Length; i++)
        {
            broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
        }
        return new IPAddress(broadcastAddress);
    }
}
