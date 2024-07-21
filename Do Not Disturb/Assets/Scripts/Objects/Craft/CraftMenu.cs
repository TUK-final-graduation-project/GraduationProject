using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    private float discountRate = 0f; // 할인가 비율

    [SerializeField] 
    public CraftTooltip tooltip;  // 툴팁 스크립트 참조

    [SerializeField]
    private Text errorMessageText; // 부족 아이템 메시지 텍스트

    void Start()
    {
        // mainCamera 변수에 현재 활성화된 메인 카메라를 할당
        cam = Camera.main;
        if (tooltip == null)
        {
            Debug.LogError("Tooltip is not assigned in the inspector.");
        }
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
        tooltip.HideTooltip();
    }

    public Craft GetCraft(int index)
    {
        if (index >= 0 && index < craftTower.Length)
        {
            return craftTower[index];
        }
        return null;
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
                    ShowErrorMessage("아이템이 부족합니다.");
                }
            }
            else
            {
                Debug.LogError("currentCraft or go_Prefab is null.");
            }
        }
    }

    // 부족 아이템 메시지 표시
    private void ShowErrorMessage(string message)
    {
        errorMessageText.text = message;
        errorMessageText.gameObject.SetActive(true);
        errorMessageText.color = Color.red;

        // 2초 후에 메시지를 비활성화
        StartCoroutine(HideErrorMessageAfterDelay(2f));
    }

    // 메세지 딜레이 후 제거
    private IEnumerator HideErrorMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        errorMessageText.gameObject.SetActive(false);
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
            // 필요한 아이템 개수에서 할인을 적용한 개수를 계산
            int discountedCount = Mathf.CeilToInt(requiredItem.count * (1 - discountRate));

            // 할인가 적용한 아이템 개수 사용
            inventory.UseItem(requiredItem.item, discountedCount);
        }
    }

    // 코인 할인
    public void ApplyDiscount(int coinNum)
    {
        // 예: 코인 숫자에 따라 할인을 5% 증가시키는 로직
        discountRate += 0.05f * coinNum;
        if (discountRate > 0.5f) // 최대 할인율 제한 (예: 50%)
        {
            discountRate = 0.5f;
        }
        Debug.Log("할인율이 적용되었습니다: " + (discountRate * 100) + "%");
    }

    // 아이템 가격을 가져올 때 할인을 적용하는 메서드
    public float GetDiscountedPrice(float originalPrice)
    {
        return originalPrice * (1 - discountRate);
    }

    // 할인율 반환해주는 메서드
    public float GetDiscountRate()
    {
        return discountRate;
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
