using UnityEngine;

public class MyAnotherPlayer : MonoBehaviour
{
    Animator animator;
    public string playerName;

    // 이동 속도 (이 값은 필요에 따라 조정할 수 있습니다)
    public float moveSpeed = 5.0f;

    // 목적지 위치
    private Vector3 targetPosition;

    void Start()
    {
        targetPosition = transform.position;
        animator = GetComponent<Animator>();    
    }

    void Update()
    {
        // 현재 위치에서 targetPosition으로 선형 보간
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        //animator.SetTrigger("Idle");
    }

    // 위치 업데이트 메서드
    public void UpdatePosition(Vector3 newPosition)
    {
        targetPosition = newPosition;
    }

    // 상태 업데이트 메서드
    public void UpdateState(State newState)
    {
        // 상태 업데이트 로직을 여기에 추가할 수 있습니다
    }
}
