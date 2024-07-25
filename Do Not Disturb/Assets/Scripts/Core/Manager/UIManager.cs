using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    public GameObject CraftTab;      
    private PlayerTools playerTools;

    void Start()
    {
        // PlayerTools ��ũ��Ʈ ���� ã��
        playerTools = FindObjectOfType<PlayerTools>();
    }

    void Update()
    {
        // ���� ���, �� Ű�� ���� UI�� ����ϴ� ���
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleUI();
        }
    }

    // UI ��� �Լ�
    public void ToggleUI()
    {
        bool isOpen = !CraftTab.activeSelf;
        CraftTab.SetActive(isOpen);
        playerTools.SetUIOpen(isOpen); // UI ���� ���� ����
    }

    // UI ���� �Լ�
    public void OpenUI()
    {
        CraftTab.SetActive(true);
        playerTools.SetUIOpen(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // UI �ݱ� �Լ�
    public void CloseUI()
    {
        CraftTab.SetActive(false);
        playerTools.SetUIOpen(false);
        Cursor.visible = false;
    }
}
