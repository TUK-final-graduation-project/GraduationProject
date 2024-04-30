// 플레이어 클래스 예시
using UnityEngine;

public class Player : MonoBehaviour
{
    private Vector3 position; // 플레이어 위치 정보

    void Update()
    {
        // 플레이어 움직임 로직
        // 예시: 키보드 입력으로 플레이어 이동
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput);
        transform.Translate(movement * Time.deltaTime * 20);

        // 플레이어 위치 업데이트
        position = transform.position;
    }

    // 서버로 플레이어 위치 정보 전송
    public Vector3 GetPosition()
    {
        return position;
    }
}
