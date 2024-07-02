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
        // Player_Movement 스크립트를 가지고 있는 게임 오브젝트를 찾습니다.
        player = FindObjectOfType<Player_Movement>();
        anotherPlayer = FindObjectOfType<AnotherPlayer>();
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
            Chat.instance.ShowMessage($"소켓에러 : {e.Message}");
        }
    }

    private float sendPositionInterval = 1f; // 1초마다 한 번 전송
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

        // 매 프레임마다 경과된 시간을 누적하고, 일정 간격이 되면 플레이어의 위치를 서버로 전송합니다.
        timeSinceLastSend += Time.deltaTime;
        if (timeSinceLastSend >= sendPositionInterval)
        {
            SendPlayerPosition();
            timeSinceLastSend = 0f; // 누적된 시간 초기화
        }
    }

    void OnIncomingData(string data)
    {
        if (data.StartsWith("%NAME"))
        {
            // 랜덤값을 사용자 이름으로 넣어준다
            clientName = NickInput.text == "" ? "Guest" + UnityEngine.Random.Range(1000, 10000) : NickInput.text;
            Send($"&NAME|{clientName}");
            return;
        }

        // 플레이어 위치 데이터 수신
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

    // 플레이어의 위치를 서버로 전송합니다.
    void SendPlayerPosition()
    {
        if (player == null) return;

        // 플레이어의 위치 정보를 JSON 형식으로 변환합니다.
        Vector3 playerPosition = player.transform.position;

        // 서버로 위치 정보를 전송합니다.
        Send(playerPosition);
    }

    void HandlePlayerPosition(string data)
    {
        // 데이터에서 위치 정보 추출
        string json = data.Split('|')[1];
        Vector3 position = JsonUtility.FromJson<Vector3>(json);

        // 수신한 위치로 플레이어 위치 업데이트
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
