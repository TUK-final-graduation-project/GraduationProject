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
        // Player_Movement 스크립트를 가지고 있는 게임 오브젝트를 찾습니다.
        player = FindObjectOfType<PlayerMovement>();
        anotherPlayer = FindObjectOfType<MyAnotherPlayer>();
    }

    public void ConnectToServer()
    {
        if (socketReady) return;

        Debug.Log("ConnectToServer() 호출");

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

    private float sendPositionInterval = 0.1f; // 0.1초마다 한 번 전송
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

        // 매 프레임마다 경과된 시간을 누적하고, 일정 간격이 되면 플레이어의 위치를 서버로 전송합니다.
        timeSinceLastSend += Time.deltaTime;
        if (timeSinceLastSend >= sendPositionInterval)
        {
            SendPlayerData();
            timeSinceLastSend = 0f; // 누적된 시간 초기화
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
                    // 랜덤값을 사용자 이름으로 넣어준다
                    clientName = NickInput.text == "" ? "Guest" + UnityEngine.Random.Range(1000, 10000) : NickInput.text;
                    SendName();
                    return;
                }

                // 맵 데이터 수신
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
