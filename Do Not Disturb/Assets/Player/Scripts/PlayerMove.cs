
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    public float speed;
    public GameObject[] tools;
    public bool[] hasTools;

    float yRotation;
    float xRotation;

    float hAxis;
    float vAxis;
    bool wDown;
    bool rDown;
    bool jDown;
    bool dDown;
    bool fDown;

    bool swapTool1;
    bool swapTool2;
    bool swapTool3;

    bool isJump;
    bool isDash;
    bool isSwap;
    bool isSwingReady = true;
    bool sward;
    bool axe;
    bool pickax;

    Vector3 moveVec;
    Vector3 dashVec;

    [SerializeField]
    Rigidbody rigid;

    [SerializeField]
    Animator anim;

    [SerializeField]
    Camera cam;

    [SerializeField]
    private float lookSensitivity;

    [SerializeField]
    private float cameraRotationLimit;
    //private float currentCameraRotationX = 0;
    //private float currentCameraRotationY = 0;


    Tools equipTool;
    //CraftMenu craftMenu;

    int equipToolIndex = -1;
    float swingDelay;
    int toolIndex = -1;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        //craftMenu = FindObjectOfType<CraftMenu>();
        
        // 마우스 커서 삭제
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void FixedUpdate()
    {
        // 키 입력 처리 함수 호출
        HandleInput();
    }

    void HandleInput()
    {
        // 이전에 있던 GetInput 함수 내용을 이동
        yRotation = Input.GetAxisRaw("Mouse X");
        xRotation = Input.GetAxisRaw("Mouse Y");

        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButton("Jump");
        dDown = Input.GetButton("Dash");
        fDown = Input.GetButton("Fire1");

        swapTool1 = Input.GetButton("Swap1");
        swapTool2 = Input.GetButton("Swap2");
        swapTool3 = Input.GetButton("Swap3");
    }

    void Update()
    {
        // 움직임 및 애니메이션 관련 함수 호출
        Move();
        Turn();
        Jump();
        Swing();
        Dash();
        Swap();

    }
    void Move()
    {
        // 카메라의 방향을 고려하여 이동 방향 벡터 계산
        Vector3 cameraForward = cam.transform.forward;
        cameraForward.y = 0; // 수평 방향으로만 이동해야 하므로 y값을 0으로 설정
        Vector3 moveDirection = cameraForward.normalized * vAxis + cam.transform.right * hAxis;

        // 이동 방향 벡터 정규화
        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        // 이동 속도 적용
        Vector3 velocity = moveDirection * speed;

        // 대시 중인 경우 대시 속도 적용
        if (isDash)
        {
            velocity = dashVec;
        }

        // 스왑 중이거나 스윙이 준비되지 않은 경우 이동 멈춤
        if (isSwap || !isSwingReady)
        {
            velocity = Vector3.zero;
        }

        // Rigidbody에 힘을 가해 이동
        rigid.MovePosition(transform.position + velocity * Time.deltaTime);

        // 이동 애니메이션 설정
        anim.SetBool("isWalk", velocity != Vector3.zero);
        anim.SetBool("isRun", rDown);

        // 이동 방향에 따라 캐릭터 회전
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }


    void Turn()
    {
        // Rotation
        transform.LookAt(transform.position + moveVec);
    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDash && !isSwap)
        {
            rigid.AddForce(Vector3.up * 5, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("Jump");
            isJump = true;
        }
    }

    void Swing()
    {
        if (equipTool == null)
            return;

        swingDelay += Time.deltaTime;
        isSwingReady = equipTool.rate < swingDelay;

        if (fDown && isSwingReady && !isDash && !isSwap)
        {
            equipTool.Use();
            
            Debug.Log("Swing");
            if (sward) anim.SetTrigger("Sward");
            if (axe) anim.SetTrigger("Axe");
            if (pickax) anim.SetTrigger("Pickax");
            swingDelay = 0;
        }
    }

    void Dash()
    {
        if (dDown && moveVec != Vector3.zero && !isJump && !isDash && !isSwap)
        {
            dashVec = moveVec;
            speed *= 2;
            //animator.SetTrigger("isDash");
            isDash = true;
            Invoke("DashOut", 0.6f);
        }
    }

    void DashOut()
    {
        isDash = false;
        speed *= 0.5f;
    }

    void Swap()
    {
        //중복교체 막음
        if (swapTool1 && (!hasTools[0] || equipToolIndex == 0))
            return;
        if (swapTool2 && (!hasTools[1] || equipToolIndex == 1))
            return;
        if (swapTool3 && (!hasTools[2] || equipToolIndex == 2))
            return;

        toolIndex = -1;
        if (swapTool1) { toolIndex = 0; sward = true; axe = false; pickax = false; }
        if (swapTool2) { toolIndex = 1; sward = false; axe = false; pickax = true; }
        if (swapTool3) {toolIndex = 2; sward = false; axe = true; pickax = false; }

        if ((swapTool1 || swapTool2 || swapTool3) && !isDash && !isJump)
        {

            //Debug.Log(toolIndex);
            equipTool?.gameObject.SetActive(false);

            equipToolIndex = toolIndex;
            equipTool = tools[toolIndex].GetComponent<Tools>();
            equipTool.gameObject.SetActive(true);

            //장착 애니메이션 활성화
            anim.SetTrigger("Swap");

            isSwap = true;

            //스왑종료 알리기
            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        Debug.Log("SwapOut");
        isSwap = false;
    }

    public int GetToolIndex()
    {
        return equipToolIndex;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            //anim.SetBool("Idle", true);
            isJump = false;
        }
    }

    private void CharacterRotation()
    {
        //Debug.Log("yRotation  : " + yRotation);
        // 좌우 캐릭터 회전
/*
        Vector3 characterRotationY = new Vector3(0f, yRotation, 0f) * lookSensitivity;
        rigid.MoveRotation(rigid.rotation * Quaternion.Euler(characterRotationY));*/
    }

    private void CameraRotation()
    {
        //if (!craftMenu.isCrafting)
        {
            //Debug.Log("상하 카메라 회전");
            // 상하 카메라 회전

           /* float cameraRotationX = xRotation * lookSensitivity;
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);*/
        }
    }


}