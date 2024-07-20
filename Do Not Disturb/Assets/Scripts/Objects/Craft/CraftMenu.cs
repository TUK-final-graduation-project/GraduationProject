using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RequiredItem
{
    public Item item;
    public int count;
}

// 각 항목에 대한 설명을 위한 Craft 클래스
[System.Serializable]
public class Craft
{
    public string craftName; // 이름
    public GameObject go_prefab; // 실제 설치 될 프리팹
    public GameObject go_PreviewPrefab; // 미리 보기 프리팹
    public List<RequiredItem> requiredItems; // 필요한 아이템 목록과 개수

}

public class CraftMenu : MonoBehaviour
{
    private bool isActivated = false;           // CraftMenu의 활성 상태를 나타내는 변수
    private bool isPreviewActivated = false;    // 미리 보기가 활성화된 상태인지를 나타내는 변수

   
    [SerializeField]
    private GameObject go_BaseUI;               // 기본 베이스 UI GameObject

    // 선택 탭 결정
    private int selectTab = -1;

    [SerializeField]                            // 각 탭에 대한 크래프트 배열
    private Craft[] craftTower;                 // 4종류

    private GameObject go_Preview;              // 미리 보기 프리팹을 담을 변수
    private GameObject go_Prefab;               // 실제 생성될 프리팹을 담을 변수
    private Craft currentCraft;

    [SerializeField]
    private Transform tf_Player;                // 플레이어의 위치를 나타내는 Transform

    
    private RaycastHit hitInfo;                 // 레이캐스트를 통해 충돌 정보를 저장할 변수

    
    [SerializeField]
    private LayerMask layerMask;                // 레이캐스트에서 검출할 레이어 마스크

    
    [SerializeField]
    private float range = 100f;                 // 레이캐스트의 최대 거리

    
    [SerializeField]
    private Camera cam;                         // 카메라

    public bool isCrafting;

    void Start()
    {
        // mainCamera 변수에 현재 활성화된 메인 카메라를 할당
        cam = Camera.main;
    }

    public void TabClick(int _tabNumber)
    {
        selectTab = _tabNumber;
        Debug.Log("탭 click: " + _tabNumber);
    }

    // 슬롯을 클릭했을 때 호출되는 함수
    public void SlotClick(int _slotNumber)
    {
        currentCraft = craftTower[_slotNumber];
        // 미리 보기 생성
        go_Preview = Instantiate(craftTower[_slotNumber].go_PreviewPrefab, tf_Player.position + tf_Player.forward, Quaternion.identity);

        // 실제 생성될 프리팹 설정
        go_Prefab = craftTower[_slotNumber].go_prefab;

        // 미리 보기 활성화
        isPreviewActivated = true;

        // 기본 베이스 UI 비활성화
        go_BaseUI.SetActive(false);
    }

    void Update()
    {
        // 탭 키를 눌렀을 때 크래프트 메뉴 열기
        if (Input.GetKeyDown(KeyCode.Tab) && !isPreviewActivated)
            Window();

        // 미리 보기가 활성화된 경우 미리 보기 위치 업데이트
        if (isPreviewActivated)
            PreviewPositionUpdate();

        // 마우스 오른쪽 버튼을 눌렀을 때 건설
        if (Input.GetButtonDown("Fire2"))
            Build();

        // ESC 키를 눌렀을 때 취소
        if (Input.GetKeyDown(KeyCode.Escape))
            Cancel();

        // isCrafting = isActivated;
    }

    // 미리 보기 위치 업데이트 함수
    private void PreviewPositionUpdate()
    {
        // 카메라의 위치와 방향을 사용하여 레이캐스트를 수행
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hitInfo, range, layerMask))
        {
            // 충돌 지점으로 미리 보기 이동
            go_Preview.transform.position = hitInfo.point;
        }
    }

    // 건설 함수
    private void Build()
    {
        // 미리 보기가 활성화되고 건설 가능한 경우에만 실행
        if (isPreviewActivated && go_Preview.GetComponent<PreviewObject>().isBuildable())
        {
            if (currentCraft != null && go_Prefab != null)
            {
                if (CheckRequiredItems(currentCraft))
                {
                    Instantiate(go_Prefab, hitInfo.point, Quaternion.identity);
                    Debug.Log("Build : " + tf_Player.transform.position);
                    Destroy(go_Preview);
                    isActivated = false;
                    isPreviewActivated = false;
                    go_Preview = null;
                    go_Prefab = null;
                    UseRequiredItems(currentCraft);
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else
                {
                    Debug.LogWarning("아이템이 부족합니다. : " + currentCraft.craftName);
                }
            }
            else
            {
                Debug.LogError("currentCraft or go_Prefab is null.");
            }
        }
    }

    private bool CheckRequiredItems(Craft craft)
    {
        Inventory inventory = FindObjectOfType<Inventory>();

        foreach (var requiredItem in craft.requiredItems)
        {
            if (!inventory.HasItem(requiredItem.item, requiredItem.count))
            {
                return false;
            }
        }
        return true;
    }

    private void UseRequiredItems(Craft craft)
    {
        Inventory inventory = FindObjectOfType<Inventory>();

        foreach (var requiredItem in craft.requiredItems)
        {
            inventory.UseItem(requiredItem.item, requiredItem.count);
        }
    }


    // 크래프트 메뉴 열기/닫기 함수
    private void Window()
    {
        if (!isActivated)
            OpenWindow();
        else
            CloseWindow();
    }

    private void OpenWindow()
    {
        // Craft Manual UI를 활성화
        isActivated = true;
        isCrafting = true;
        go_BaseUI.SetActive(true);
        Cursor.visible = true;
        // 마우스 커서 고정 해제
        Cursor.lockState = CursorLockMode.None;
    }

    private void CloseWindow()
    {
        // Craft Manual UI를 비활성화
        isActivated = false;
        go_BaseUI.SetActive(false);

        // 마우스 커서 삭제
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // 취소 함수
    private void Cancel()
    {
        // 미리 보기가 활성화된 경우 삭제 및 상태 초기화
        if (isPreviewActivated)
            Destroy(go_Preview);

        isActivated = false;
        isCrafting = false;
        isPreviewActivated = false;
        go_Preview = null;
        go_Prefab = null;
        go_BaseUI.SetActive(false);

        // 마우스 커서 삭제
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
