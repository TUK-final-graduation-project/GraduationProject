using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.IO;
using System;

public class Client : MonoBehaviour
{
    public InputField IPInput, PortInput, NickInput;
    string clientName;

    bool socketReady;
    TcpClient socket;
    NetworkStream stream;
    StreamWriter writer;
    StreamReader reader;
    Player_Movement player;
    AnotherPlayer anotherPlayer;

    public Vector3 playerPosition;

    void Start()
    {
        // Player_Movement ��ũ��Ʈ�� ������ �ִ� ���� ������Ʈ�� ã���ϴ�.
        player = FindObjectOfType<Player_Movement>();
        anotherPlayer = FindObjectOfType<AnotherPlayer>();
    }

    public void ConnectToServer()
    {
        if (socketReady) return;

        Debug.Log("ConnectToServer() ȣ��");

        string ip = IPInput.text == "" ? "127.0.0.1" : IPInput.text;
        int port = PortInput.text == "" ? 7777 : int.Parse(PortInput.text);

        try
        {
            socket = new TcpClient(ip, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            socketReady = true;
        }
        catch (Exception e)
        {
            Chat.instance.ShowMessage($"���Ͽ��� : {e.Message}");
        }
    }

    private float sendPositionInterval = 1f; // 1�ʸ��� �� �� ����
    private float timeSinceLastSend = 0f;

    void Update()
    {
        if (socketReady && stream.DataAvailable)
        {
            string data = reader.ReadLine();
            if (data != null)
            {
                OnIncomingData(data);
            }
        }

        // �� �����Ӹ��� ����� �ð��� �����ϰ�, ���� ������ �Ǹ� �÷��̾��� ��ġ�� ������ �����մϴ�.
        timeSinceLastSend += Time.deltaTime;
        if (timeSinceLastSend >= sendPositionInterval)
        {
            SendPlayerPosition();
            timeSinceLastSend = 0f; // ������ �ð� �ʱ�ȭ
        }
    }

    void OnIncomingData(string data)
    {
        if (data.StartsWith("%NAME"))
        {
            // �������� ����� �̸����� �־��ش�
            clientName = NickInput.text == "" ? "Guest" + UnityEngine.Random.Range(1000, 10000) : NickInput.text;
            Send($"&NAME|{clientName}");
            return;
        }

        // �÷��̾� ��ġ ������ ����
        if (data.StartsWith("&POSITION"))
        {
            HandlePlayerPosition(data);
            return;
        }

        Chat.instance.ShowMessage(data);
    }

    void Send(string data)
    {
        if (!socketReady) return;

        writer.WriteLine(data);
        writer.Flush();
    }

    void Send(Vector3 position)
    {
        if (!socketReady) return;

        string positionData = JsonUtility.ToJson(position);
        writer.WriteLine($"&POSITION|{positionData}");
        writer.Flush();
    }

    public void OnSendButton(InputField SendInput)
    {
#if (UNITY_EDITOR || UNITY_STANDALONE)
        if (!Input.GetButtonDown("Submit")) return;
        SendInput.ActivateInputField();
#endif
        if (SendInput.text.Trim() == "") return;

        string message = SendInput.text;
        SendInput.text = "";
        Send(message);
    }

    // �÷��̾��� ��ġ�� ������ �����մϴ�.
    void SendPlayerPosition()
    {
        if (player == null) return;

        // �÷��̾��� ��ġ ������ JSON �������� ��ȯ�մϴ�.
        Vector3 playerPosition = player.transform.position;

        // ������ ��ġ ������ �����մϴ�.
        Send(playerPosition);
    }

    void HandlePlayerPosition(string data)
    {
        // �����Ϳ��� ��ġ ���� ����
        string json = data.Split('|')[1];
        Vector3 position = JsonUtility.FromJson<Vector3>(json);

        // ������ ��ġ�� �÷��̾� ��ġ ������Ʈ
        if (anotherPlayer != null)
        {
            anotherPlayer.UpdatePosition(position);
        }
    }

    void OnApplicationQuit()
    {
        CloseSocket();
    }

    void CloseSocket()
    {
        if (!socketReady) return;

        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }

    public Vector3 getPlayerPosition()
    {
        return playerPosition;
    }
}
