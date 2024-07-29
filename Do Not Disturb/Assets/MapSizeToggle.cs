using UnityEngine;

public class MapSizeToggle : MonoBehaviour
{
    public Vector3 enlargedSize = new Vector3(3, 3, 3); // 확대된 크기
    public Vector3 originalSize = new Vector3(1, 1, 1); // 원래 크기
    public Vector3 centerPosition = new Vector3(-510, -250, 0); // 중심 위치

    private Vector3 originalPosition; // 원래 위치를 저장
    private bool isEnlarged = false; // 현재 확대 상태를 저장

    void Start()
    {
        // 게임 오브젝트의 초기 위치를 저장
        originalPosition = transform.position;
    }

    // Update는 매 프레임마다 호출됩니다.
    void Update()
    {
        // R 키를 누를 때마다 크기를 토글
        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleSize();
        }
    }

    // 크기를 토글하는 함수
    void ToggleSize()
    {
        if (isEnlarged)
        {
            // 원래 크기와 위치로 되돌림
            transform.localScale = originalSize;
            transform.position = originalPosition;
        }
        else
        {
            // 확대 크기와 중심 위치로 변경
            transform.localScale = enlargedSize;
            transform.position = originalPosition+centerPosition;
        }

        isEnlarged = !isEnlarged; // 상태 토글
    }
}
