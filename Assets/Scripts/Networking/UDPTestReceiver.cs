using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UdpClientExample : MonoBehaviour
{
    private UdpClient udpClient;
    private Thread receiveThread;

    public int port = 1111;
    public bool start = false;

    private volatile string lastMessage; // <-- thread-safe handoff

    void Start()
    {
        Debug.Log("UDP listener started on port " + port);

        // Get your Wi-Fi adapter IP explicitly
        var wifiIP = IPAddress.Parse("192.168.160.185");

        udpClient = new UdpClient(AddressFamily.InterNetwork);
        udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        udpClient.Client.Bind(new IPEndPoint(wifiIP, port));

        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }



    void ReceiveData()
    {
        try
        {
            Debug.Log("Receive thread started");
            IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                Debug.Log("Waiting for UDP packet...");
                byte[] data = udpClient.Receive(ref anyIP);
                lastMessage = Encoding.UTF8.GetString(data).Trim();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("UDP thread error: " + e);
        }

    }

    void Update()
    {
        if (!string.IsNullOrEmpty(lastMessage))
        {
            Debug.Log("Received: " + lastMessage);

            if (lastMessage == "Start")
            {
                start = true;
            }

            lastMessage = null; // clear after processing
        }
    }

    void OnApplicationQuit()
    {
        receiveThread?.Abort();
        udpClient?.Close();
    }
}
