using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    public GameObject CraftTab;      
    private PlayerTools playerTools;

    void Start()
    {
        // PlayerTools 스크립트 참조 찾기
        playerTools = FindObjectOfType<PlayerTools>();
    }

    void Update()
    {
        // 예를 들어, 탭 키를 눌러 UI를 토글하는 기능
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleUI();
        }
    }

    // UI 토글 함수
    public void ToggleUI()
    {
        bool isOpen = !CraftTab.activeSelf;
        CraftTab.SetActive(isOpen);
        playerTools.SetUIOpen(isOpen); // UI 열림 상태 설정
    }

    // UI 열기 함수
    public void OpenUI()
    {
        CraftTab.SetActive(true);
        playerTools.SetUIOpen(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // UI 닫기 함수
    public void CloseUI()
    {
        CraftTab.SetActive(false);
        playerTools.SetUIOpen(false);
        Cursor.visible = false;
    }
}
