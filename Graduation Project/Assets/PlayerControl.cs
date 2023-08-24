using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float lookSensitivity;

    [SerializeField]
    private Camera firstPersonViewCamera;

    [SerializeField]
    private Camera topViewCamera;

    [SerializeField]
    private Camera quaterViewCamera;

    private Rigidbody myRigid;

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.tag == "topView") // 충돌한 오브젝트의 태그 확인
        {
            firstPersonViewCamera.enabled = false; // 1인칭 카메라 비활성화
            topViewCamera.enabled = true;     // Top 뷰 카메라 활성화
            quaterViewCamera.enabled = false;
        }

        if (other.gameObject.tag == "firstPersonView") // 충돌이 끝난 오브젝트의 태그 확인
        {
            firstPersonViewCamera.enabled = true;  // 1인칭 카메라 활성화
            topViewCamera.enabled = false;     // Top 뷰 카메라 비활성화
            quaterViewCamera.enabled = false;
        }

        if (other.gameObject.tag == "quaterView") // 충돌이 끝난 오브젝트의 태그 확인
        {
            firstPersonViewCamera.enabled = true;  // 1인칭 카메라 활성화
            topViewCamera.enabled = false;     // Top 뷰 카메라 비활성화
            quaterViewCamera.enabled = true;

        }
    }

    // Start is called before the first frame update
    void Start()
    {
        myRigid = GetComponent<Rigidbody>();
        firstPersonViewCamera.enabled = false;
        topViewCamera.enabled = true;
        quaterViewCamera.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        CharacterRotation();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 첫 번째 카메라를 활성화하고, 두 번째 카메라를 비활성화
            firstPersonViewCamera.enabled = true;
            topViewCamera.enabled = false;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            // 두 번째 카메라를 활성화하고, 첫 번째 카메라를 비활성화
            firstPersonViewCamera.enabled = false;
            topViewCamera.enabled = true;
        }
    }

    private void Move()
    {
        float moveDirectionX = Input.GetAxisRaw("Horizontal");
        float moveDirectionZ = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * moveDirectionX;
        Vector3 moveVertical = transform.forward * moveDirectionZ;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * walkSpeed;

        myRigid.MovePosition(transform.position + velocity * Time.deltaTime);
    }

    private void CharacterRotation() 
    {
        //좌우 캐릭터 회전
        float yRotaion = Input.GetAxisRaw("Mouse X");
        Vector3 characterRotationY = new Vector3(0f, yRotaion, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(characterRotationY));

    }
}
