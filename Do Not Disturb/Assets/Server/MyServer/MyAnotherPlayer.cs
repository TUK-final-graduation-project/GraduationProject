using UnityEngine;

public class MyAnotherPlayer : MonoBehaviour
{
    public string playerName;

    // �̵� �ӵ� (�� ���� �ʿ信 ���� ������ �� �ֽ��ϴ�)
    public float moveSpeed = 5.0f;

    // ������ ��ġ
    private Vector3 targetPosition;

    void Start()
    {
        targetPosition = transform.position;
    }

    void Update()
    {
        // ���� ��ġ���� targetPosition���� ���� ����
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
    }

    // ��ġ ������Ʈ �޼���
    public void UpdatePosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
    }

    // ���� ������Ʈ �޼���
    public void UpdateState(State newState)
    {
        // ���� ������Ʈ ������ ���⿡ �߰��� �� �ֽ��ϴ�
    }
}
