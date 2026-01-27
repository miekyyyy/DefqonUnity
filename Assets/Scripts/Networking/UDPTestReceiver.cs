using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace DefqonEngine
{
    public class UDPTestReceiver : MonoBehaviour
    {
        public bool start = false;
        public int listenPort = 1111;
        public UdpClient udpServer;
        public IPEndPoint remoteEP;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // Initialize server
            udpServer = new UdpClient(listenPort);
            remoteEP = new IPEndPoint(IPAddress.Any, 0);
        }

        // Update is called once per frame
        void Update()
        {
            try
            {
                // Wait for message
                byte[] receivedMessage = udpServer.Receive(ref remoteEP);

                // Convert byte[] to readable string
                string message = Encoding.UTF8.GetString(receivedMessage);

                // Check if its a start message
                if (message != null && message == "Start");
                {
                    start = true;
                    Console.WriteLine("Received start message");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
