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
    private Craft[] craftTower;     // WATER, FIRE, EARTH, FOREST

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

    [SerializeField]
    private Camera cam;

    public bool isCrafting;

    void Start()
    {
        // mainCamera ������ ���� Ȱ��ȭ�� ���� ī�޶� �Ҵ��մϴ�.
        // cam = Camera.main;
    }

    // ������ Ŭ������ �� ȣ��Ǵ� �Լ�
    public void SlotClick(int _slotNumber)
    {
        // �̸� ���� ����
        go_Preview = Instantiate(craftTower[_slotNumber].go_PreviewPrefab, tf_Player.position + tf_Player.forward, Quaternion.identity);

        // ���� ������ ������ ����
        go_Prefab = craftTower[_slotNumber].go_prefab;

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
        //if (isPreviewActivated)
        PreviewPositionUpdate();

        // ���콺 ������ ��ư�� ������ �� �Ǽ�
        if (Input.GetButtonDown("Fire2"))
            Build();

        // ESC Ű�� ������ �� ���
        if (Input.GetKeyDown(KeyCode.Escape))
            Cancel();

        isCrafting = isActivated;
    }

    // �̸� ���� ��ġ ������Ʈ �Լ�
    private void PreviewPositionUpdate()
    {
        // ī�޶��� ��ġ�� ������ ����Ͽ� ����ĳ��Ʈ�� ����
        //if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, range, layerMask))
        {
            Vector3 playerPosition = tf_Player.transform.position; // tf_Player�� ��ġ
            Vector3 playerForward = tf_Player.transform.forward; // tf_Player�� ���� ����
            float distance = 1.5f; // �Ÿ�

            // tf_Player�� ��ġ���� ���� �������� �Ÿ� 5��ŭ ������ ��ġ ���
            Vector3 targetPosition = playerPosition + playerForward * distance;

            //if (hitInfo.transform != null)
            {
                // �浹 �������� �̸� ���� �̵�
                //go_Preview.transform.position = targetPosition;
            }
        }
    }

    // �Ǽ� �Լ�
    private void Build()
    {
        // �̸� ���Ⱑ Ȱ��ȭ�ǰ� �Ǽ� ������ ��쿡�� ����
        if (isPreviewActivated /*&& go_Preview.GetComponent<PreviewObject>().isBuildable()*/)
        {
            // go_Prefab�� null�� �ƴ��� Ȯ��
            if (go_Prefab != null)
            {
                Debug.Log("diq : " + tf_Player.transform.position);

                Vector3 playerPosition = tf_Player.transform.position; // tf_Player�� ��ġ
                Vector3 playerForward = tf_Player.transform.forward; // tf_Player�� ���� ����
                float distance = 1.5f; // �Ÿ�

                // tf_Player�� ��ġ���� ���� �������� �Ÿ� 5��ŭ ������ ��ġ ���
                Vector3 targetPosition = playerPosition + playerForward * distance;
                // ���� ������ ����
                Instantiate(go_Prefab, targetPosition, Quaternion.identity);

                // �̸� ���� ���� �� ���� �ʱ�ȭ
                Destroy(go_Preview);
                isActivated = false;
                isCrafting = false;
                isPreviewActivated = false;
                go_Preview = null;
                go_Prefab = null;
            }
            else
            {
                Debug.LogError("go_Prefab�� null�Դϴ�.");
            }
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
        isCrafting = true;
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
        isCrafting = false;
        isPreviewActivated = false;
        go_Preview = null;
        go_Prefab = null;
        go_BaseUI.SetActive(false);
    }
}
