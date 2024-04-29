
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
    private float currentCameraRotationX = 0;


    Tools equipTool;
    //CraftMenu craftMenu;

    int equipToolIndex = -1;
    float swingDelay;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        //craftMenu = FindObjectOfType<CraftMenu>();
        // 마우스 커서 삭제

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Swing();
        Dash();
        Swap();
        CameraRotation();
        CharacterRotation();
    }

    void GetInput()
    {
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

    void Move()
    {
        //moveVec = new Vector3(hAxis, 0, vAxis).normalized;


        Vector3 moveHorizontal = transform.right * hAxis;
        Vector3 moveVertical = transform.forward * vAxis;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * speed;

        if (isDash)
            velocity = dashVec;

        if (isSwap || !isSwingReady)
            velocity = Vector3.zero;

        //rigid.AddForce(new Vector3(hAxis, 0, vAxis), ForceMode.Impulse);
        //rigid.MovePosition(transform.position + velocity * Time.deltaTime);
        rigid.AddForce(velocity * Time.deltaTime, ForceMode.VelocityChange);
        // animator
        anim.SetBool("isWalk", velocity != Vector3.zero);
        anim.SetBool("isRun", rDown);
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
            anim.SetTrigger("Pickax");
            Debug.Log("Swing");
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

        int toolIndex = -1;
        if (swapTool1) toolIndex = 0;
        if (swapTool2) toolIndex = 1;
        if (swapTool3) toolIndex = 2;

        if ((swapTool1 || swapTool2 || swapTool3) && !isDash && !isJump)
        {

            //Debug.Log(toolIndex);
            equipTool?.gameObject.SetActive(false);

            equipToolIndex = toolIndex;
            equipTool = tools[toolIndex].GetComponent<Tools>();
            equipTool.gameObject.SetActive(true);

            //장착 애니메이션 활성화
            // animator.SetTrigger("doSwap");
            anim.SetTrigger("Attack");

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
        //Debug.Log("좌우 캐릭터 회전");
        // 좌우 캐릭터 회전

        Vector3 characterRotationY = new Vector3(0f, yRotation, 0f) * lookSensitivity;
        rigid.MoveRotation(rigid.rotation * Quaternion.Euler(characterRotationY));

    }

    private void CameraRotation()
    {
        //if (!craftMenu.isCrafting)
        {
            //Debug.Log("상하 카메라 회전");
            // 상하 카메라 회전

            float cameraRotationX = xRotation * lookSensitivity;
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }
    }


}