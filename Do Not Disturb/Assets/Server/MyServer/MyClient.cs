using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.IO;
using System;

public class MyClient : MonoBehaviour
{
    public InputField IPInput, PortInput, NickInput;
    string clientName;
    string clientID = Guid.NewGuid().ToString(); // Unique identifier for this client

    bool socketReady;
    TcpClient socket;
    NetworkStream stream;
    StreamWriter writer;
    StreamReader reader;
    PlayerMovement player;
    MyAnotherPlayer anotherPlayer;

    MapData mapData;

    void Start()
    {
        // Player_Movement ��ũ��Ʈ�� ������ �ִ� ���� ������Ʈ�� ã���ϴ�.
        player = FindObjectOfType<PlayerMovement>();
        anotherPlayer = FindObjectOfType<MyAnotherPlayer>();
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
            Chat.instance.ShowMessage($"���� ����: {e.Message}");
        }
    }

    private float sendPositionInterval = 0.1f; // 0.1�ʸ��� �� �� ����
    private float timeSinceLastSend = 0f;

    void Update()
    {
        if (socketReady && stream.DataAvailable)
        {
            byte[] data = new byte[1024];
            int bytesRead = stream.Read(data, 0, data.Length);
            if (bytesRead > 0)
            {
                OnIncomingData(data, bytesRead);
            }
        }

        // �� �����Ӹ��� ����� �ð��� �����ϰ�, ���� ������ �Ǹ� �÷��̾��� ��ġ�� ������ �����մϴ�.
        timeSinceLastSend += Time.deltaTime;
        if (timeSinceLastSend >= sendPositionInterval)
        {
            SendPlayerData();
            timeSinceLastSend = 0f; // ������ �ð� �ʱ�ȭ
        }
    }

    void OnIncomingData(byte[] data, int bytesRead)
    {
        using (MemoryStream ms = new MemoryStream(data, 0, bytesRead))
        {
            using (BinaryReader reader = new BinaryReader(ms))
            {
                string dataType = reader.ReadString();
                if (dataType == "%NAME")
                {
                    // �������� ����� �̸����� �־��ش�
                    clientName = NickInput.text == "" ? "Guest" + UnityEngine.Random.Range(1000, 10000) : NickInput.text;
                    SendName();
                    return;
                }

                // �� ������ ����
                if (dataType == "&MAPDATA")
                {
                    int playerCount = reader.ReadInt32();
                    mapData = new MapData
                    {
                        playerData = new List<PlayerData>()
                    };
                    for (int i = 0; i < playerCount; i++)
                    {
                        string clientID = reader.ReadString();
                        float posX = reader.ReadSingle();
                        float posY = reader.ReadSingle();
                        float posZ = reader.ReadSingle();
                        State state = (State)reader.ReadInt32();

                        PlayerData playerData = new PlayerData
                        {
                            clientID = clientID,
                            position = new Vector3(posX, posY, posZ),
                            state = state
                        };
                        mapData.playerData.Add(playerData);
                    }
                    UpdateAnotherPlayer();
                    return;
                }
            }
        }
    }

    void SendName()
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write("&NAME");
                writer.Write(clientName);
                SendData(ms.ToArray());
            }
        }
    }

    void SendPlayerData()
    {
        if (player == null) return;

        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write("&PLAYERDATA");
                writer.Write(clientID);
                writer.Write(player.transform.position.x);
                writer.Write(player.transform.position.y);
                writer.Write(player.transform.position.z);
                writer.Write((int)player.state);
                SendData(ms.ToArray());
            }
        }
    }

    void SendData(byte[] data)
    {
        if (!socketReady) return;

        stream.Write(data, 0, data.Length);
        stream.Flush();
    }

    void UpdateAnotherPlayer()
    {
        if (anotherPlayer == null || mapData == null) return;

        foreach (var playerData in mapData.playerData)
        {
            if (playerData.clientID != clientID)
            {
                anotherPlayer.UpdatePosition(playerData.position);
                anotherPlayer.UpdateState(playerData.state);
            }
        }
    }
}
