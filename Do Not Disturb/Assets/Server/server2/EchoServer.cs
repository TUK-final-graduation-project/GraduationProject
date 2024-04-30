// ���� �ڵ� ����
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
        Console.WriteLine("���� ����: 127.0.0.1:4444");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("Ŭ���̾�Ʈ �����: " + client.Client.RemoteEndPoint);

            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine("���� �޽���: " + message);

            // �÷��̾��� ������ ������ �޽����� ����
            Player player = new Player(); // �÷��̾� ��ü ����
            Vector3 playerPosition = player.GetPosition();
            string playerMovement = $"Player1: ({playerPosition.x}, {playerPosition.y}, {playerPosition.z})";
            byte[] response = Encoding.UTF8.GetBytes(playerMovement);
            stream.Write(response, 0, response.Length);

            client.Close();
        }
    }
}
