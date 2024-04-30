using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class EchoServer
{
    public static void Main()
    {
        // 서버 소켓 설정
        TcpListener server = null;
        try
        {
            // 로컬호스트의 13000번 포트에서 클라이언트 연결 대기
            Int32 port = 13000;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");
            server = new TcpListener(localAddr, port);

            // 서버 시작
            server.Start();

            Console.WriteLine("서버 시작, 클라이언트 연결 대기 중...");

            // 최대 3명의 클라이언트 연결 대기
            for (int i = 0; i < 3; i++)
            {
                // 클라이언트 연결 대기
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("클라이언트가 연결되었습니다.");

                // 클라이언트와 통신을 위한 새로운 스레드 생성
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
            // 서버 종료
            server.Stop();
        }

        Console.WriteLine("\n서버가 종료되었습니다.");
        Console.ReadLine();
    }

    // 클라이언트와 통신하는 메서드
    public static void HandleClient(object clientObj)
    {
        TcpClient client = (TcpClient)clientObj;

        // 클라이언트와 연결된 소켓에서 데이터 수신
        Byte[] bytes = new Byte[256];
        String data = null;
        NetworkStream stream = client.GetStream();

        int i;
        // 데이터를 읽고 클라이언트에 에코
        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
        {
            // 데이터를 바이트 배열에서 문자열로 변환
            data = Encoding.ASCII.GetString(bytes, 0, i);
            Console.WriteLine("수신: {0}", data);

            // 데이터를 다시 클라이언트에 전송
            byte[] msg = Encoding.ASCII.GetBytes(data);
            stream.Write(msg, 0, msg.Length);
            Console.WriteLine("송신: {0}", data);
        }

        // 클라이언트 연결 종료
        client.Close();
    }
}
