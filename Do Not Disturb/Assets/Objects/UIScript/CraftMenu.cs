using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 각 항목에 대한 설명을 위한 Craft 클래스
[System.Serializable]
public class Craft
{
    public string craftName; // 이름
    public GameObject go_prefab; // 실제 설치 될 프리팹
    public GameObject go_PreviewPrefab; // 미리 보기 프리팹
}

public class CraftMenu : MonoBehaviour
{
    // CraftMenu의 활성 상태를 나타내는 변수
    private bool isActivated = false;

    // 미리 보기가 활성화된 상태인지를 나타내는 변수
    private bool isPreviewActivated = false;

    // 기본 베이스 UI GameObject
    [SerializeField]
    private GameObject go_BaseUI;

    // 각 탭에 대한 크래프트 배열
    [SerializeField]
    private Craft[] craftTower;     // WATER, FIRE, EARTH, FOREST

    // 미리 보기 프리팹을 담을 변수
    private GameObject go_Preview;

    // 실제 생성될 프리팹을 담을 변수
    private GameObject go_Prefab;

    // 플레이어의 위치를 나타내는 Transform
    [SerializeField]
    private Transform tf_Player;

    // 레이캐스트를 통해 충돌 정보를 저장할 변수
    private RaycastHit hitInfo;

    // 레이캐스트에서 검출할 레이어 마스크
    [SerializeField]
    private LayerMask layerMask;

    // 레이캐스트의 최대 거리
    [SerializeField]
    private float range;

    [SerializeField]
    private Camera cam;

    public bool isCrafting;

    void Start()
    {
        // mainCamera 변수에 현재 활성화된 메인 카메라를 할당합니다.
        // cam = Camera.main;
    }

    // 슬롯을 클릭했을 때 호출되는 함수
    public void SlotClick(int _slotNumber)
    {
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
        //if (isPreviewActivated)
        PreviewPositionUpdate();

        // 마우스 오른쪽 버튼을 눌렀을 때 건설
        if (Input.GetButtonDown("Fire2"))
            Build();

        // ESC 키를 눌렀을 때 취소
        if (Input.GetKeyDown(KeyCode.Escape))
            Cancel();

        isCrafting = isActivated;
    }

    // 미리 보기 위치 업데이트 함수
    private void PreviewPositionUpdate()
    {
        // 카메라의 위치와 방향을 사용하여 레이캐스트를 수행
        //if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, range, layerMask))
        {
            Vector3 playerPosition = tf_Player.transform.position; // tf_Player의 위치
            Vector3 playerForward = tf_Player.transform.forward; // tf_Player의 전방 벡터
            float distance = 1.5f; // 거리

            // tf_Player의 위치에서 전방 방향으로 거리 5만큼 떨어진 위치 계산
            Vector3 targetPosition = playerPosition + playerForward * distance;

            //if (hitInfo.transform != null)
            {
                // 충돌 지점으로 미리 보기 이동
                //go_Preview.transform.position = targetPosition;
            }
        }
    }

    // 건설 함수
    private void Build()
    {
        // 미리 보기가 활성화되고 건설 가능한 경우에만 실행
        if (isPreviewActivated /*&& go_Preview.GetComponent<PreviewObject>().isBuildable()*/)
        {
            // go_Prefab이 null이 아닌지 확인
            if (go_Prefab != null)
            {
                Debug.Log("diq : " + tf_Player.transform.position);

                Vector3 playerPosition = tf_Player.transform.position; // tf_Player의 위치
                Vector3 playerForward = tf_Player.transform.forward; // tf_Player의 전방 벡터
                float distance = 1.5f; // 거리

                // tf_Player의 위치에서 전방 방향으로 거리 5만큼 떨어진 위치 계산
                Vector3 targetPosition = playerPosition + playerForward * distance;
                // 실제 프리팹 생성
                Instantiate(go_Prefab, targetPosition, Quaternion.identity);

                // 미리 보기 삭제 및 상태 초기화
                Destroy(go_Preview);
                isActivated = false;
                isCrafting = false;
                isPreviewActivated = false;
                go_Preview = null;
                go_Prefab = null;
            }
            else
            {
                Debug.LogError("go_Prefab이 null입니다.");
            }
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
    }
}
