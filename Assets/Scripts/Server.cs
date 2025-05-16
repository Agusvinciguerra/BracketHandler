using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class Server : MonoBehaviour
{
    private TcpListener server;
    private Thread serverThread;
    private List<TcpClient> clients = new List<TcpClient>();
    [SerializeField] private int puerto;
    [SerializeField] private TMP_InputField portInput;

    /*void Start()
    {
        serverThread = new Thread(StartServer);
        serverThread.IsBackground = true;
        serverThread.Start();
    }*/

    public void InitializeServer()
    {
        int portToUse = puerto;
        if (portInput != null && !string.IsNullOrEmpty(portInput.text))
        {
            int.TryParse(portInput.text, out portToUse);
        }

        serverThread = new Thread(() => StartServer(portToUse));
        serverThread.IsBackground = true;
        serverThread.Start();
    }

    void StartServer(int port)
    {
        server = new TcpListener(IPAddress.Any, port);
        server.Start();
        Debug.Log("Server started on port " + port);

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            lock (clients)
            {
                clients.Add(client);
            }
            Debug.Log("Client connected!");

            // Start a thread to handle this client
            new Thread(() => HandleClient(client)).Start();
        }
    }

    void HandleClient(TcpClient client)
    {
        try
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[4096];
            while (true)
            {
                int length = stream.Read(buffer, 0, buffer.Length);
                if (length == 0) break; // Client disconnected

                string message = Encoding.UTF8.GetString(buffer, 0, length);
                Debug.Log("Received: " + message);

                // Broadcast to all other clients
                lock (clients)
                {
                    foreach (var c in clients)
                    {
                        if (c != client && c.Connected)
                        {
                            try
                            {
                                Debug.Log("Enviando mensaje a cliente: " + c.Client.RemoteEndPoint);
                                NetworkStream s = c.GetStream();
                                byte[] msgBytes = Encoding.UTF8.GetBytes(message);
                                s.Write(msgBytes, 0, msgBytes.Length);
                            }
                            catch { /* Ignore errors */ }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Client error: " + ex.Message);
        }
        finally
        {
            lock (clients)
            {
                clients.Remove(client);
            }
            client.Close();
        }
    }

    void OnApplicationQuit()
    {
        server?.Stop();
        serverThread?.Abort();
    }
}