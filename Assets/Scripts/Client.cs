using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;

public class Client : MonoBehaviour
{
    TcpClient client;
    Thread receiveThread;
    [SerializeField] private TMP_InputField portInput;
    public int puerto;

    public void ConnectToServer()
    {
        // EX START METHOD //
        /*try
        {
            client = new TcpClient("127.0.0.1", puerto);
            Debug.Log("Client connected to server!");
            StartReceiveThread();
        }
        catch (SocketException ex)
        {
            Debug.LogError("Client could not connect to server: " + ex.Message);
        }*/

        int portToUse = puerto;
        if (portInput != null && !string.IsNullOrEmpty(portInput.text))
        {
            int.TryParse(portInput.text, out portToUse);
        }

        try
        {
            client = new TcpClient("127.0.0.1", portToUse);
            Debug.Log("Client connected to server on port: " + portToUse);
            StartReceiveThread();
        }
        catch (SocketException ex)
        {
            Debug.LogError("Client could not connect to server: " + ex.Message);
        }
    }

    public void SendMessageToServer(string msg)
    {
        NetworkStream stream = client.GetStream();
        byte[] data = Encoding.UTF8.GetBytes(msg);
        stream.Write(data, 0, data.Length);
        Debug.Log("Message sent: " + msg);
    }

    void StartReceiveThread()
    {
        receiveThread = new Thread(ReceiveData);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void ReceiveData()
    {
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[4096];
        while (true)
        {
            int length = stream.Read(buffer, 0, buffer.Length);
            if (length == 0) break;
            string message = Encoding.UTF8.GetString(buffer, 0, length);

            Debug.Log("Mensaje recibido en Client: " + message);

            // Llama a UIHandler en el hilo principal
            MainThreadDispatcher.Enqueue(() =>
            {
                FindObjectOfType<UIHandler>()?.OnMessageReceived(message);
            });
        }
    }

    void OnApplicationQuit()
    {
        client?.Close();
        receiveThread?.Abort();
    }
}