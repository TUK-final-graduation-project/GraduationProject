using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

class EchoClient : MonoBehaviour
{
    static void Main()
    {
        TcpClient client = new TcpClient("127.0.0.1", 4444);
        NetworkStream stream = client.GetStream();

        // 서버로부터 메시지 수신
        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string serverMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine("서버 메시지: " + serverMessage);

        // 플레이어의 움직임 정보 파싱 및 업데이트
        float x, y, z;
        if (serverMessage.StartsWith("Player1:"))
        {
            string[] parts = serverMessage.Split(':');
            if (parts.Length == 2)
            {
                string[] coordinates = parts[1].Trim().TrimStart('(').TrimEnd(')').Split(',');
                if (coordinates.Length == 3)
                {
                    float.TryParse(coordinates[0], out x);
                    float.TryParse(coordinates[1], out y);
                    float.TryParse(coordinates[2], out z);
                    Console.WriteLine($"플레이어 위치: x={x}, y={y}, z={z}");
                }
            }
        }

        client.Close();
    }
}
