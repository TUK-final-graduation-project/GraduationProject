using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed;

    [SerializeField]
    private float jumpPower;

    [SerializeField]
    private float lookSensitivity;

    [SerializeField]
    private Camera firstPersonViewCamera;

    [SerializeField]
    private Camera topViewCamera;

    [SerializeField]
    private Camera quaterViewCamera;

    private Rigidbody myRigid;

    float moveDirectionX;
    float moveDirectionZ;

    public GameObject[] PickItem;
    public bool[] hasPickItem;

    bool isJumping;
    public bool isPicking;

    GameObject nearObject;
    GameObject equipPoint;

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ground")
        {
            isJumping = false;
        }
    }

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

        // ---------------- 아이템
        if (other.gameObject.tag == "Item")
        {
            nearObject = other.gameObject;
            Debug.Log(nearObject.name);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.tag == "Item")
        {
            nearObject = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        myRigid = GetComponent<Rigidbody>();
        equipPoint = GameObject.FindGameObjectWithTag("EquipPoint");
        firstPersonViewCamera.enabled = false;
        topViewCamera.enabled = true;
        quaterViewCamera.enabled = false;
    }

    // 키 입력
    void Interaction()
    {
        if (Input.GetButtonDown("Fire3") && nearObject != null)
        {
            Debug.Log("interaction");
            if (nearObject.tag == "Item")
            {
                ItemAction item = nearObject.GetComponent<ItemAction>();
                int itemIndex = item.value;             // value는 아이템의 정보를 담음 : 1-던지기만 가능...
                hasPickItem[itemIndex] = true;
                Destroy(nearObject);
            }
        }

    }

    void Update()
    {
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

        moveDirectionX = Input.GetAxisRaw("Horizontal");
        moveDirectionZ = Input.GetAxisRaw("Vertical");

        //Jumping
        if (Input.GetButtonDown("Jump"))
            isJumping = true;

        //Dropping
        if (Input.GetButtonUp("Fire2") && isPicking)
        {
            Debug.Log("Dropping");
            Drop();
        }

        Interaction();

    }

    // 고정적 물리 처리
    void FixedUpdate()
    {
        Move();
        Jump();
        CharacterRotation();

    }

    private void Jump()
    {
        if (!isJumping)
            return;

        myRigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        isJumping = false;
    }

    private void Move()
    {
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

    public void Pickup(GameObject item)
    {
        SetEquip(item, true);

        isPicking = true;
    }

    void Drop()
    {

        GameObject item = equipPoint.GetComponentInChildren<Rigidbody>().gameObject;
        SetEquip(item, false);

        equipPoint.transform.DetachChildren();
        isPicking = false;
    }

    void SetEquip(GameObject item, bool isEquip)
    {
        Collider[] itemColliders = item.GetComponents<Collider>();
        Rigidbody itemRigidbody = item.GetComponent<Rigidbody>();

        foreach (Collider itemCollider in itemColliders)
        {
            itemCollider.enabled = !isEquip;
        }

        itemRigidbody.isKinematic = isEquip;
    }
}
