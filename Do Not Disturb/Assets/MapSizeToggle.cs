using UnityEngine;

public class MapSizeToggle : MonoBehaviour
{
    public Vector3 enlargedSize = new Vector3(3, 3, 3); // Ȯ��� ũ��
    public Vector3 originalSize = new Vector3(1, 1, 1); // ���� ũ��
    public Vector3 centerPosition = new Vector3(-510, -250, 0); // �߽� ��ġ

    private Vector3 originalPosition; // ���� ��ġ�� ����
    private bool isEnlarged = false; // ���� Ȯ�� ���¸� ����

    void Start()
    {
        // ���� ������Ʈ�� �ʱ� ��ġ�� ����
        originalPosition = transform.position;
    }

    // Update�� �� �����Ӹ��� ȣ��˴ϴ�.
    void Update()
    {
        // R Ű�� ���� ������ ũ�⸦ ���
        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleSize();
        }
    }

    // ũ�⸦ ����ϴ� �Լ�
    void ToggleSize()
    {
        if (isEnlarged)
        {
            // ���� ũ��� ��ġ�� �ǵ���
            transform.localScale = originalSize;
            transform.position = originalPosition;
        }
        else
        {
            // Ȯ�� ũ��� �߽� ��ġ�� ����
            transform.localScale = enlargedSize;
            transform.position = originalPosition+centerPosition;
        }

        isEnlarged = !isEnlarged; // ���� ���
    }
}
