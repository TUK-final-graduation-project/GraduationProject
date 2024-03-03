using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �� �׸� ���� ������ ���� Craft Ŭ����
[System.Serializable]
public class Craft
{
    public string craftName; // �̸�
    public GameObject go_prefab; // ���� ��ġ �� ������
    public GameObject go_PreviewPrefab; // �̸� ���� ������
}

public class CraftMenu : MonoBehaviour
{
    // CraftMenu�� Ȱ�� ���¸� ��Ÿ���� ����
    private bool isActivated = false;

    // �̸� ���Ⱑ Ȱ��ȭ�� ���������� ��Ÿ���� ����
    private bool isPreviewActivated = false;

    // �⺻ ���̽� UI GameObject
    [SerializeField]
    private GameObject go_BaseUI;

    // �� �ǿ� ���� ũ����Ʈ �迭
    [SerializeField]
    private Craft[] craftWaterTower;

    // �̸� ���� �������� ���� ����
    private GameObject go_Preview;

    // ���� ������ �������� ���� ����
    private GameObject go_Prefab;

    // �÷��̾��� ��ġ�� ��Ÿ���� Transform
    [SerializeField]
    private Transform tf_Player;

    // ����ĳ��Ʈ�� ���� �浹 ������ ������ ����
    private RaycastHit hitInfo;

    // ����ĳ��Ʈ���� ������ ���̾� ����ũ
    [SerializeField]
    private LayerMask layerMask;

    // ����ĳ��Ʈ�� �ִ� �Ÿ�
    [SerializeField]
    private float range;

    // ������ Ŭ������ �� ȣ��Ǵ� �Լ�
    public void SlotClick(int _slotNumber)
    {
        // �̸� ���� ����
        go_Preview = Instantiate(craftWaterTower[_slotNumber].go_PreviewPrefab, tf_Player.position + tf_Player.forward, Quaternion.identity);

        // ���� ������ ������ ����
        go_Prefab = craftWaterTower[_slotNumber].go_prefab;

        // �̸� ���� Ȱ��ȭ
        isPreviewActivated = true;

        // �⺻ ���̽� UI ��Ȱ��ȭ
        go_BaseUI.SetActive(false);
    }

    void Update()
    {
        // �� Ű�� ������ �� ũ����Ʈ �޴� ����
        if (Input.GetKeyDown(KeyCode.Tab) && !isPreviewActivated)
            Window();

        // �̸� ���Ⱑ Ȱ��ȭ�� ��� �̸� ���� ��ġ ������Ʈ
        if (isPreviewActivated)
            PreviewPositionUpdate();

        // ���콺 ������ ��ư�� ������ �� �Ǽ�
        if (Input.GetButtonDown("Fire2"))
            Build();

        // ESC Ű�� ������ �� ���
        if (Input.GetKeyDown(KeyCode.Escape))
            Cancel();
    }

    // �̸� ���� ��ġ ������Ʈ �Լ�
    private void PreviewPositionUpdate()
    {
        // ����ĳ��Ʈ�� ���� �浹 Ȯ��
        if (Physics.Raycast(tf_Player.position, tf_Player.forward, out hitInfo, range, layerMask))
        {
            if (hitInfo.transform != null)
            {
                // �浹 �������� �̸� ���� �̵�
                go_Preview.transform.position = hitInfo.point;
            }
        }
    }

    // �Ǽ� �Լ�
    private void Build()
    {
        // �̸� ���Ⱑ Ȱ��ȭ�ǰ� �Ǽ� ������ ��쿡�� ����
        if (isPreviewActivated && go_Preview.GetComponent<PreviewObject>().isBuildable())
        {
            // ���� ������ ����
            Instantiate(go_Prefab, hitInfo.point, Quaternion.identity);

            // �̸� ���� ���� �� ���� �ʱ�ȭ
            Destroy(go_Preview);
            isActivated = false;
            isPreviewActivated = false;
            go_Preview = null;
            go_Prefab = null;
        }
    }

    // ũ����Ʈ �޴� ����/�ݱ� �Լ�
    private void Window()
    {
        if (!isActivated)
            OpenWindow();
        else
            CloseWindow();
    }

    private void OpenWindow()
    {
        // Craft Manual UI�� Ȱ��ȭ
        isActivated = true;
        go_BaseUI.SetActive(true);
        Cursor.visible = true;
        // ���콺 Ŀ�� ���� ����
        Cursor.lockState = CursorLockMode.None;
    }

    private void CloseWindow()
    {
        // Craft Manual UI�� ��Ȱ��ȭ
        isActivated = false;
        go_BaseUI.SetActive(false);
        Cursor.visible = false;
    }

    // ��� �Լ�
    private void Cancel()
    {
        // �̸� ���Ⱑ Ȱ��ȭ�� ��� ���� �� ���� �ʱ�ȭ
        if (isPreviewActivated)
            Destroy(go_Preview);

        isActivated = false;
        isPreviewActivated = false;
        go_Preview = null;
        go_Prefab = null;
        go_BaseUI.SetActive(false);
    }
}
