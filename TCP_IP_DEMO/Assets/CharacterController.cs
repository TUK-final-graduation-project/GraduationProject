using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;

public class CharacterController : MonoBehaviour
{
    // ���� �ּҿ� ��Ʈ
    public string serverAddress = "127.0.0.1";
    public int serverPort = 13000;

    // TCP Ŭ���̾�Ʈ ����
    private TcpClient client;
    private NetworkStream stream;
    private byte[] receiveBuffer;

    // ĳ������ Transform ������Ʈ
    private Transform characterTransform;

    // Start �޼��忡�� ������ ����
    void Start()
    {
        try
        {
            // ������ ����
            client = new TcpClient(serverAddress, serverPort);
            stream = client.GetStream();

            // ������ ������ ���� ���� ����
            receiveBuffer = new byte[1024];

            // �����κ��� �񵿱������� ������ ����
            stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveCallback, null);

            Debug.Log("������ ����Ǿ����ϴ�.");

            // ĳ������ Transform ������Ʈ ����
            characterTransform = GetComponent<Transform>();
        }
        catch (Exception e)
        {
            Debug.LogError("������ ������ �� �����ϴ�: " + e);
        }
    }

    // Update �޼��忡�� ĳ������ ��ġ ������ ������ ����
    void Update()
    {
        // ĳ������ ��ġ ������ ���ڿ��� ��ȯ�Ͽ� ������ ����
        string positionData = characterTransform.position.x + "," + characterTransform.position.y + "," + characterTransform.position.z;
        SendMessageToServer(positionData);
    }

    // ������ �޽��� ������ �޼���
    public void SendMessageToServer(string message)
    {
        try
        {
            // ���ڿ��� ����Ʈ �迭�� ��ȯ�Ͽ� ������ ����
            byte[] data = Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Debug.Log("�۽�: " + message);
        }
        catch (Exception e)
        {
            Debug.LogError("������ �޽����� ���� �� �����ϴ�: " + e);
        }
    }

    // �����κ��� ���ŵ� ������ ó�� �ݹ� �޼���
    private void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            int bytesRead = stream.EndRead(ar);
            if (bytesRead <= 0)
            {
                return;
            }

            // ���ŵ� �����͸� ���ڿ��� ��ȯ�Ͽ� ���
            string receivedMessage = Encoding.ASCII.GetString(receiveBuffer, 0, bytesRead);
            Debug.Log("����: " + receivedMessage);

            // �ٽ� �����κ��� �񵿱������� ������ ����
            stream.BeginRead(receiveBuffer, 0, receiveBuffer.Length, ReceiveCallback, null);
        }
        catch (Exception e)
        {
            Debug.LogError("������ ���� �� ���� �߻�: " + e);
        }
    }

    // ���� ���� �� Ŭ���̾�Ʈ ������ ����
    private void OnDestroy()
    {
        if (client != null)
        {
            client.Close();
        }
    }
}
