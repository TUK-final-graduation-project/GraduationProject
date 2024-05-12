using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public float runSpeed = 8.0f;
    public float finalSpeed;
    public float smoothness = 10.0f;

    Animator anim;
    Camera cam;
    CharacterController controll;
    Rigidbody rigid;

    public bool toggleCameraRotation;
    public bool isRun;
    public bool isJump;

    bool jDown;         // space bar 입력


    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
        cam = Camera.main;
        controll = this.GetComponent<CharacterController>();
        rigid = this.GetComponent<Rigidbody>();
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


        // 점프
        //Jump();
        InputMovement();
    }

    void LateUpdate()
    {
        if (toggleCameraRotation != true)
        {
            Vector3 playerRotate = Vector3.Scale(cam.transform.forward, new Vector3(1, 0, 1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
        }
    }

    void InputMovement()
    {

        finalSpeed = (isRun) ? runSpeed : speed;
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        Vector3 moveDirection = forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal");

        controll.Move(moveDirection.normalized * finalSpeed * Time.deltaTime);
        float percent = ((isRun) ? 1 : 0.5f) * moveDirection.magnitude;
        anim.SetFloat("Blend", percent, 0.5f, Time.deltaTime);
    }

    void Jump()
    {
        if (jDown && !isJump)
        {
            rigid.AddForce(Vector3.up * 5, ForceMode.Impulse);
            anim.SetBool("isJump", true);
            anim.SetTrigger("Jump");
            isJump = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            //anim.SetBool("Idle", true);
            isJump = false;
        }
    }

}
