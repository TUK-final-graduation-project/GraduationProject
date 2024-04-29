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

        // �����κ��� �޽��� ����
        byte[] buffer = new byte[1024];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        string serverMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine("���� �޽���: " + serverMessage);

        // �÷��̾��� ������ ���� �Ľ� �� ������Ʈ
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
                    Console.WriteLine($"�÷��̾� ��ġ: x={x}, y={y}, z={z}");
                }
            }
        }

        client.Close();
    }
}
