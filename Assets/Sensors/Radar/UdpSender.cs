using UnityEngine;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class UdpSender : MonoBehaviour {

    public int port = 6000;
    public string ipAddress = "127.0.0.1";
    public float frequence = 0.0f;

    //private string data = string.Empty;
    private Queue data = new Queue(60,10);
    private byte[] buffer;
    private Thread sender;

    private bool isSending = true;

    private bool data_updated = false;

    public void SetData(string data)
    {
        this.data.Enqueue(data);
        data_updated = true;
    }

    void Sender()
    {
        using (UdpClient client = new UdpClient(ipAddress, port)){
            while (isSending)
            {
                if (data_updated)
                {
                    string data = this.data.Dequeue().ToString();
                    buffer = Encoding.ASCII.GetBytes(data);
                    client.Send(buffer, buffer.Length);
                    if (this.data.Count==0)
                    {
                        data_updated = false;
                    }
                }
//                Thread.Sleep((int)(1.0f / frequence * 1000));
            }
        }
    }

    IEnumerator SenderStart()
    {
        ThreadStart senderStart = new ThreadStart(Sender);
        sender = new Thread(senderStart);
        sender.IsBackground = true;
        sender.Start();
        yield return null;
    }

    void Start()
    {
        isSending = true;
        StartCoroutine("SenderStart");
    }

    void OnDestroy()
    {
        Exit();
    }

    void Exit()
    {
        isSending = false;
        if (sender != null)
            if (sender.IsAlive)
                sender.Abort();

    }

    void OnApplicationQuit()
    {
        Exit();
    }
}