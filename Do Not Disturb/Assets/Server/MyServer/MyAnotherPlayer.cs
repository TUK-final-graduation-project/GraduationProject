using UnityEngine;

public class MyAnotherPlayer : MonoBehaviour
{
    Animator animator;
    public string playerName;

    // �̵� �ӵ� (�� ���� �ʿ信 ���� ������ �� �ֽ��ϴ�)
    public float moveSpeed = 5.0f;

    // ������ ��ġ
    private Vector3 targetPosition;

    void Start()
    {
        targetPosition = transform.position;
        animator = GetComponent<Animator>();    
    }

    void Update()
    {
        // ���� ��ġ���� targetPosition���� ���� ����
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        //animator.SetTrigger("Idle");
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
