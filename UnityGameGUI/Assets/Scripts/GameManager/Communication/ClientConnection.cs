using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class ClientConnection
{
    private ClientWebSocket clientWS = new ClientWebSocket();
    private UTF8Encoding encoder;

    //Server address
    private Uri serverUri;

    // Queues
    private BlockingCollection<ArraySegment<byte>> sendQueue { get; }
    public ConcurrentQueue<string> receiveQueue { get; }

    // Threads
    private Thread sendThread { get; set; }
    private Thread receiveThread { get; set; }

    //Constructor
    public ClientConnection(string serverURL)
    {
        encoder = new UTF8Encoding();
        clientWS = new ClientWebSocket();
        serverUri = new Uri(serverURL);

        receiveQueue = new ConcurrentQueue<string>();
        sendQueue = new BlockingCollection<ArraySegment<byte>>();

        sendThread = new Thread(SendRunning);
        receiveThread = new Thread(ReceiveRunning);
        
    }

    public void Connect()
    {
        Debug.Log("Connecting to: " + serverUri);
        clientWS.ConnectAsync(serverUri, CancellationToken.None);
        while (IsConnecting())
        {
            Debug.Log("Waiting to connect...");
            Task.Delay(50).Wait();
        }
        Debug.Log("Connect status: " + clientWS.State);

        sendThread.Start();
        receiveThread.Start();
    }

    public async Task Disconnect()
    {
        await clientWS.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
        while (clientWS.State != WebSocketState.Closed)
        {
            Debug.Log("Waiting to disconnect...");
            Task.Delay(50).Wait();
        }
        Debug.Log("Disconnected");
    }

    //Add new message to the queue
    public void SendMessage(string message)
    {
        Debug.Log("Send Message: " + message);
        byte[] buffer = encoder.GetBytes(message);
        var sendBuf = new ArraySegment<byte>(buffer);
        
        sendQueue.Add(sendBuf);
    }

    //Send Message Thread running method
    private async void SendRunning()
    {
        while (IsConnectionOpen())
        {
            ArraySegment<byte> msg;
            while (!sendQueue.IsCompleted)
            {
                msg = sendQueue.Take();

                await clientWS.SendAsync(msg, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
        Debug.Log("Thread finished");
    }

    //Get message from server and conver to string
    private async Task<string> Receive()
    {
        // A read buffer, and a memory stream to stuff unknown number of chunks into:
        byte[] buf = new byte[4 * 1024];
        var ms = new MemoryStream();
        ArraySegment<byte> arrayBuf = new ArraySegment<byte>(buf);
        WebSocketReceiveResult chunkResult = null;
        if (IsConnectionOpen())
        {
            do
            {
                chunkResult = await clientWS.ReceiveAsync(arrayBuf, CancellationToken.None);
                ms.Write(arrayBuf.Array, arrayBuf.Offset, chunkResult.Count);
            } while (!chunkResult.EndOfMessage);

            ms.Seek(0, SeekOrigin.Begin);

            // Looking for UTF-8 JSON type messages.
            if (chunkResult.MessageType == WebSocketMessageType.Text)
            {
                StreamReader reader = new StreamReader(ms, Encoding.UTF8);
                string readString = reader.ReadToEnd();
                Debug.Log("Received Message: " + readString);
                return readString;
            }
        }
        return "";
    }

    //Receive Message Thread running method
    private async void ReceiveRunning()
    {
        string result;

        while (true)
        {
            result = await Receive();
            if (result != null && result.Length > 0)
            {
                receiveQueue.Enqueue(result);
            }
            else
            {
                Task.Delay(50).Wait();
            }
        }
        Debug.Log("Thread finished");
    }

    public bool IsConnectionOpen()
    {
        return clientWS.State == WebSocketState.Open;
    }

    public bool IsConnecting()
    {
        return clientWS.State == WebSocketState.Connecting;
    }
}