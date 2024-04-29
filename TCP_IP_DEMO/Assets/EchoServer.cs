using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class EchoServer
{
    public static void Main()
    {
        // ���� ���� ����
        TcpListener server = null;
        try
        {
            // ����ȣ��Ʈ�� 13000�� ��Ʈ���� Ŭ���̾�Ʈ ���� ���
            Int32 port = 13000;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            server = new TcpListener(localAddr, port);

            // ���� ����
            server.Start();

            Console.WriteLine("���� ����, Ŭ���̾�Ʈ ���� ��� ��...");

            // �ִ� 3���� Ŭ���̾�Ʈ ���� ���
            for (int i = 0; i < 3; i++)
            {
                // Ŭ���̾�Ʈ ���� ���
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Ŭ���̾�Ʈ�� ����Ǿ����ϴ�.");

                // Ŭ���̾�Ʈ�� ����� ���� ���ο� ������ ����
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClient));
                clientThread.Start(client);
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            // ���� ����
            server.Stop();
        }

        Console.WriteLine("\n������ ����Ǿ����ϴ�.");
        Console.ReadLine();
    }

    // Ŭ���̾�Ʈ�� ����ϴ� �޼���
    public static void HandleClient(object clientObj)
    {
        TcpClient client = (TcpClient)clientObj;

        // Ŭ���̾�Ʈ�� ����� ���Ͽ��� ������ ����
        Byte[] bytes = new Byte[256];
        String data = null;
        NetworkStream stream = client.GetStream();

        int i;
        // �����͸� �а� Ŭ���̾�Ʈ�� ����
        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
        {
            // �����͸� ����Ʈ �迭���� ���ڿ��� ��ȯ
            data = Encoding.ASCII.GetString(bytes, 0, i);
            Console.WriteLine("����: {0}", data);

            // �����͸� �ٽ� Ŭ���̾�Ʈ�� ����
            byte[] msg = Encoding.ASCII.GetBytes(data);
            stream.Write(msg, 0, msg.Length);
            Console.WriteLine("�۽�: {0}", data);
        }

        // Ŭ���̾�Ʈ ���� ����
        client.Close();
    }
}
