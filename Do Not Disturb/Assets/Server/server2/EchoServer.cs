// 서버 코드 예시
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

class EchoServer : MonoBehaviour
{
    static void Main()
    {
        TcpListener server = new TcpListener(IPAddress.Any, 4444);
        server.Start();
        Console.WriteLine("서버 시작: 127.0.0.1:4444");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("클라이언트 연결됨: " + client.Client.RemoteEndPoint);

            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine("수신 메시지: " + message);

            // 플레이어의 움직임 정보를 메시지로 전송
            Player player = new Player(); // 플레이어 객체 생성
            Vector3 playerPosition = player.GetPosition();
            string playerMovement = $"Player1: ({playerPosition.x}, {playerPosition.y}, {playerPosition.z})";
            byte[] response = Encoding.UTF8.GetBytes(playerMovement);
            stream.Write(response, 0, response.Length);

            client.Close();
        }
    }
}
