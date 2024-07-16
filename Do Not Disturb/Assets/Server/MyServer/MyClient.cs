using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System;
using UnityEngine.UI;
using UnityEngine;

public class MyClient : MonoBehaviour
{
    public InputField IPInput, PortInput, NickInput;
    string clientName;
    string clientID = Guid.NewGuid().ToString();

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
        player = FindObjectOfType<PlayerMovement>();
        anotherPlayer = FindObjectOfType<MyAnotherPlayer>();
    }

    public void ConnectToServer()
    {
        if (socketReady) return;

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
            Chat.instance.ShowMessage($"소켓 에러: {e.Message}");
        }
    }

    private float sendPositionInterval = 0.1f;
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

        timeSinceLastSend += Time.deltaTime;
        if (timeSinceLastSend >= sendPositionInterval)
        {
            SendPlayerData();
            timeSinceLastSend = 0f;
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
                    clientName = NickInput.text == "" ? "Guest" + UnityEngine.Random.Range(1000, 10000) : NickInput.text;
                    SendName();
                    return;
                }

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
                        float dirX = reader.ReadSingle();
                        float dirY = reader.ReadSingle();
                        float dirZ = reader.ReadSingle();
                        State state = (State)reader.ReadInt32();

                        PlayerData playerData = new PlayerData
                        {
                            clientID = clientID,
                            position = new Vector3(posX, posY, posZ),
                            direction = new Vector3(dirX, dirY, dirZ),
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
                writer.Write(player.transform.forward.x);
                writer.Write(player.transform.forward.y);
                writer.Write(player.transform.forward.z);
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
                anotherPlayer.UpdatePosition(playerData.position, playerData.direction);
                anotherPlayer.UpdateState(playerData.state);
            }
        }
    }
}
