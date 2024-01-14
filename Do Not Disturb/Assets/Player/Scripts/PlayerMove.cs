using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float speed;
    public GameObject[] tools;
    public bool[] hasTools;

    float hAxis;
    float vAxis;
    bool wDown;
    bool jDown;
    bool fDown;

    bool swapTool1;
    bool swapTool2;

    bool isJump;
    bool isDodge;
    bool isSwap;
    bool isFireReady = true;

    Vector3 moveVec;
    Vector3 dodgeVec;

    Rigidbody rigid;
    // Animator animator;

    Tools equipTool;
    int equipToolIndex = -1;                // 0:도끼 1:곡괭이
    float fireDelay;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        //animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        GetInput();
        Move();
        Turn();
        Jump();
        Swing();
        Dodge();
        Swap();
    }

    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        wDown = Input.GetButton("Walk");
        jDown = Input.GetButton("Jump");
        fDown = Input.GetButton("Fire1");

        swapTool1 = Input.GetButton("Swap1");
        swapTool2 = Input.GetButton("Swap2");
    }

    void Move()
    {
        moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        if (isDodge)
            moveVec = dodgeVec;

        if (isSwap || !isFireReady)
            moveVec = Vector3.zero;

        transform.position += moveVec * speed * (wDown ? 0.8f : 1f) * Time.deltaTime;

        // animator.SetBool("isWalk", moveVec != Vector3.zero);
        // animator.SetBool("isWalk", wDown);
    }

    void Turn()
    {
        transform.LookAt(transform.position + moveVec);
    }

    void Jump()
    {
        if (jDown && moveVec == Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            rigid.AddForce(Vector3.up * 5, ForceMode.Impulse);
            //animator.SetBool("isJump", true);
            //animator.SetTrigger("isJump");
            isJump = true;
        }
    }

    void Swing()
    {
        if (equipTool == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = equipTool.rate < fireDelay;

        if(fDown && isFireReady && !isDodge && !isSwap)
        {
            equipTool.Use();
            //animation.SetTrigger("doSwing");
            Debug.Log("Swing");
            fireDelay = 0;
        }
    }

    void Dodge()
    {
        if (jDown && moveVec != Vector3.zero && !isJump && !isDodge && !isSwap)
        {
            dodgeVec = moveVec;
            speed *= 2;
            //animator.SetTrigger("isDodge");
            isDodge = true;
            Invoke("DodgeOut", 0.6f);
        }
    }

    void DodgeOut()
    {
        isDodge = false;
        speed *= 0.5f;
    }

    void Swap()
    {
        //중복교체 막음
        if (swapTool1 && (!hasTools[0] || equipToolIndex == 0))
            return;
        if (swapTool2 && (!hasTools[1] || equipToolIndex == 1))
            return;

        int toolIndex = -1;
        if (swapTool1) toolIndex = 0;
        if (swapTool2) toolIndex = 1;

        if ((swapTool1 || swapTool2) && !isDodge && !isJump)
        {

            //Debug.Log(toolIndex);
            equipTool?.gameObject.SetActive(false);

            equipToolIndex = toolIndex;
            equipTool = tools[toolIndex].GetComponent<Tools>();
            equipTool.gameObject.SetActive(true);

            //장착 애니메이션 활성화
            // animator.SetTrigger("doSwap");

            isSwap = true;

            //스왑종료 알리기
            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        //Debug.Log("SwapOut");
        isSwap = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            // animator.SetBool("Idle ", false);
            isJump = false;
        }
    }
}