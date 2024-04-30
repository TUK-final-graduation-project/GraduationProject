// �÷��̾� Ŭ���� ����
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector3 position; // �÷��̾� ��ġ ����

    void Update()
    {
        // �÷��̾� ������ ����
        // ����: Ű���� �Է����� �÷��̾� �̵�
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput);
        transform.Translate(movement * Time.deltaTime * 20);

        // �÷��̾� ��ġ ������Ʈ
        position = transform.position;
    }

    // ������ �÷��̾� ��ġ ���� ����
    public Vector3 GetPosition()
    {
        return position;
    }
}
