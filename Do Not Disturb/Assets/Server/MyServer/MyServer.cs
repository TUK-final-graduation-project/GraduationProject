using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System;
using UnityEngine.UI;
using UnityEngine;

public class MyServer : MonoBehaviour
{
    public InputField PortInput;

    List<MyServerClient> clients;
    List<MyServerClient> disconnectList;

    TcpListener server;
    bool serverStarted;

    MapData mapData;

    public void ServerCreate()
    {
        clients = new List<MyServerClient>();
        disconnectList = new List<MyServerClient>();
        mapData = new MapData
        {
            items = new List<ItemData>(),
            towers = new List<TowerData>(),
            playerData = new List<PlayerData>(),
            unitData = new List<UnitData>()
        };

        try
        {
            int port = PortInput.text == "" ? 7777 : int.Parse(PortInput.text);
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            StartListening();
            serverStarted = true;
            Chat.instance.ShowMessage($"서버가 {port}에서 시작되었습니다.");
        }
        catch (Exception e)
        {
            Chat.instance.ShowMessage($"소켓 에러: {e.Message}");
        }
    }

    void Update()
    {
        if (!serverStarted) return;

        foreach (MyServerClient c in clients)
        {
            if (!IsConnected(c.tcp))
            {
                c.tcp.Close();
                disconnectList.Add(c);
                continue;
            }
            else
            {
                NetworkStream s = c.tcp.GetStream();
                if (s.DataAvailable)
                {
                    byte[] data = new byte[1024];
                    int bytesRead = s.Read(data, 0, data.Length);
                    if (bytesRead > 0)
                        OnIncomingData(c, data, bytesRead);
                }
            }
        }

        for (int i = 0; i < disconnectList.Count; i++)
        {
            Broadcast($"{disconnectList[i].clientName} 연결이 끊어졌습니다", clients);

            clients.Remove(disconnectList[i]);
            disconnectList.RemoveAt(i);
        }
    }

    bool IsConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead))
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);

                return true;
            }
            else
                return false;
        }
        catch
        {
            return false;
        }
    }

    void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server);
    }

    void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        clients.Add(new MyServerClient(listener.EndAcceptTcpClient(ar)));
        StartListening();

        Broadcast("%NAME", new List<MyServerClient>() { clients[clients.Count - 1] });
    }

    void OnIncomingData(MyServerClient c, byte[] data, int bytesRead)
    {
        using (MemoryStream ms = new MemoryStream(data, 0, bytesRead))
        {
            using (BinaryReader reader = new BinaryReader(ms))
            {
                string dataType = reader.ReadString();
                if (dataType == "&NAME")
                {
                    c.clientName = reader.ReadString();
                    Broadcast($"{c.clientName}이 연결되었습니다", clients);
                    return;
                }

                if (dataType == "&PLAYERDATA")
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
                    UpdatePlayerData(playerData);
                    BroadcastMapData();
                    return;
                }
            }
        }

        Broadcast($"{c.clientName} : {System.Text.Encoding.UTF8.GetString(data, 0, bytesRead)}", clients);
    }

    void UpdatePlayerData(PlayerData playerData)
    {
        var existingPlayer = mapData.playerData.Find(p => p.clientID == playerData.clientID);
        if (existingPlayer != null)
        {
            existingPlayer.position = playerData.position;
            existingPlayer.direction = playerData.direction;
            existingPlayer.state = playerData.state;
        }
        else
        {
            mapData.playerData.Add(playerData);
        }
    }

    void BroadcastMapData()
    {
        using (MemoryStream ms = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write("&MAPDATA");
                writer.Write(mapData.playerData.Count);
                foreach (var player in mapData.playerData)
                {
                    writer.Write(player.clientID);
                    writer.Write(player.position.x);
                    writer.Write(player.position.y);
                    writer.Write(player.position.z);
                    writer.Write(player.direction.x);
                    writer.Write(player.direction.y);
                    writer.Write(player.direction.z);
                    writer.Write((int)player.state);
                }
                Broadcast(ms.ToArray(), clients);
            }
        }
    }

    void Broadcast(byte[] data, List<MyServerClient> cl)
    {
        foreach (var c in cl)
        {
            try
            {
                NetworkStream stream = c.tcp.GetStream();
                stream.Write(data, 0, data.Length);
                stream.Flush();
            }
            catch (Exception e)
            {
                Chat.instance.ShowMessage($"쓰기 에러: {e.Message}를 클라이언트 {c.clientName}에게");
            }
        }
    }

    void Broadcast(string data, List<MyServerClient> cl)
    {
        byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(data);
        Broadcast(dataBytes, cl);
    }
}

public class MyServerClient
{
    public TcpClient tcp;
    public string clientName;

    public MyServerClient(TcpClient clientSocket)
    {
        clientName = "Guest";
        tcp = clientSocket;
    }
}
