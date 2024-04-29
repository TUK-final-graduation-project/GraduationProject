using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;

public class CharacterController : MonoBehaviour
{
    // 서버 주소와 포트
    public string serverAddress = "127.0.0.1";
    public int serverPort = 13000;

    // TCP 클라이언트 소켓
    private TcpClient client;
    private NetworkStream stream;
    private byte[] receiveBuffer;

    // 캐릭터의 Transform 컴포넌트
    private Transform characterTransform;

    // Start 메서드에서 서버에 연결
    void Start()
    {
        try
        {
            // 서버에 연결
            client = new TcpClient(serverAddress, serverPort);
            stream = client.GetStream();

            // 데이터 수신을 위한 버퍼 설정
            receiveBuffer = new byte[1024];

            // 서버로부터 비동기적으로 데이터 수신
            stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveCallback, null);

            Debug.Log("서버에 연결되었습니다.");

            // 캐릭터의 Transform 컴포넌트 참조
            characterTransform = GetComponent<Transform>();
        }
        catch (Exception e)
        {
            Debug.LogError("서버에 연결할 수 없습니다: " + e);
        }
    }

    // Update 메서드에서 캐릭터의 위치 정보를 서버로 전송
    void Update()
    {
        // 캐릭터의 위치 정보를 문자열로 변환하여 서버로 전송
        string positionData = characterTransform.position.x + "," + characterTransform.position.y + "," + characterTransform.position.z;
        SendMessageToServer(positionData);
    }

    // 서버로 메시지 보내는 메서드
    public void SendMessageToServer(string message)
    {
        try
        {
            // 문자열을 바이트 배열로 변환하여 서버로 전송
            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Debug.Log("송신: " + message);
        }
        catch (Exception e)
        {
            Debug.LogError("서버에 메시지를 보낼 수 없습니다: " + e);
        }
    }

    // 서버로부터 수신된 데이터 처리 콜백 메서드
    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            int bytesRead = stream.EndRead(ar);
            if (bytesRead <= 0)
            {
                return;
            }

            // 수신된 데이터를 문자열로 변환하여 출력
            string receivedMessage = Encoding.ASCII.GetString(receiveBuffer, 0, bytesRead);
            Debug.Log("수신: " + receivedMessage);

            // 다시 서버로부터 비동기적으로 데이터 수신
            stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveCallback, null);
        }
        catch (Exception e)
        {
            Debug.LogError("데이터 수신 중 오류 발생: " + e);
        }
    }

    // 게임 종료 시 클라이언트 소켓을 닫음
    private void OnDestroy()
    {
        if (client != null)
        {
            client.Close();
        }
    }
}
