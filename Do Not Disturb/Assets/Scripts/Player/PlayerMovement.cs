using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public float runSpeed = 8.0f;
    public float smoothness = 10.0f;
    public float accelerationTime = 0.5f;
    public float decelerationTime = 0.5f;
    public float jumpForce = 5.0f;
    public float gravity = -9.8f;
    public float animationThreshold = 1f; // 애니메이션이 재생될 속도 임계값

    private Animator anim;
    private Camera cam;
    private CharacterController controller;

    public bool toggleCameraRotation;
    public bool isRun;
    public bool isJump;

    private bool jDown;
    private Vector3 currentVelocity;
    private Vector3 targetVelocity;
    private float currentSpeed;
    private Vector3 yVelocity; // y축 속도 관리

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        cam = Camera.main;
        controller = GetComponent<CharacterController>();

        // Ensure all components are attached
        if (anim == null)
        {
            Debug.LogError("Animator component is missing on " + gameObject.name);
        }
        if (controller == null)
        {
            Debug.LogError("CharacterController component is missing on " + gameObject.name);
        }
    }

    void FixedUpdate()
    {
        // 키 입력 처리 
        jDown = Input.GetButton("Jump");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            toggleCameraRotation = true;
        }
        else
        {
            toggleCameraRotation = false;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRun = true;
        }
        else
        {
            isRun = false;
        }

        InputMovement();
    }

    void LateUpdate()
    {
        if (!toggleCameraRotation)
        {
            Vector3 playerRotate = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
        }
    }

    void InputMovement()
    {
        if (controller == null || anim == null) return; 

        float finalSpeed = isRun ? runSpeed : speed;
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        Vector3 moveDirection = forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal");
        targetVelocity = moveDirection.normalized * finalSpeed;

        // Smooth
        if (targetVelocity.magnitude > 0)
        {
            currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.deltaTime * (1.0f / accelerationTime));
        }
        else
        {
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, Time.deltaTime * (1.0f / decelerationTime));
        }

        // Jump
        if (controller.isGrounded)
        { 
            //Debug.Log("Grounded");
            yVelocity.y = -2f; // 작은 값을 설정하여 지면에 붙게 함
            if (jDown)
            {
                yVelocity.y = jumpForce;
                isJump = true;
                Debug.Log("Jump");
                anim.SetTrigger("Jump"); // 점프 애니메이션 트리거
            }
        }
        else
        {
            yVelocity.y += gravity * Time.deltaTime;
        }

        // 전체 이동 벡터 계산
        Vector3 finalMove = currentVelocity * Time.deltaTime;
        finalMove.y = yVelocity.y * Time.deltaTime;

        controller.Move(finalMove);

        currentSpeed = currentVelocity.magnitude;
        if (currentSpeed > animationThreshold)
        {
            float percent = ((isRun) ? 1 : 0.5f) * (currentSpeed / finalSpeed);
            anim.SetFloat("Blend", percent, 0.5f, Time.deltaTime);
        }
        else
        {
            anim.SetFloat("Blend", 0, 0.5f, Time.deltaTime);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (controller.isGrounded)
        {
            isJump = false; // 착지 시 점프 상태 초기화
        }
    }
}
