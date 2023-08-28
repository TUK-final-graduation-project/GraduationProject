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
        if (other.gameObject.tag == "topView") // �浹�� ������Ʈ�� �±� Ȯ��
        {
            firstPersonViewCamera.enabled = false; // 1��Ī ī�޶� ��Ȱ��ȭ
            topViewCamera.enabled = true;     // Top �� ī�޶� Ȱ��ȭ
            quaterViewCamera.enabled = false;
        }

        if (other.gameObject.tag == "firstPersonView") // �浹�� ���� ������Ʈ�� �±� Ȯ��
        {
            firstPersonViewCamera.enabled = true;  // 1��Ī ī�޶� Ȱ��ȭ
            topViewCamera.enabled = false;     // Top �� ī�޶� ��Ȱ��ȭ
            quaterViewCamera.enabled = false;
        }

        if (other.gameObject.tag == "quaterView") // �浹�� ���� ������Ʈ�� �±� Ȯ��
        {
            firstPersonViewCamera.enabled = true;  // 1��Ī ī�޶� Ȱ��ȭ
            topViewCamera.enabled = false;     // Top �� ī�޶� ��Ȱ��ȭ
            quaterViewCamera.enabled = true;

        }

        // ---------------- ������
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

    // Ű �Է�
    void Interaction()
    {
        if (Input.GetButtonDown("Fire3") && nearObject != null)
        {
            Debug.Log("interaction");
            if (nearObject.tag == "Item")
            {
                ItemAction item = nearObject.GetComponent<ItemAction>();
                int itemIndex = item.value;             // value�� �������� ������ ���� : 1-�����⸸ ����...
                hasPickItem[itemIndex] = true;
                Destroy(nearObject);
            }
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // ù ��° ī�޶� Ȱ��ȭ�ϰ�, �� ��° ī�޶� ��Ȱ��ȭ
            firstPersonViewCamera.enabled = true;
            topViewCamera.enabled = false;
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            // �� ��° ī�޶� Ȱ��ȭ�ϰ�, ù ��° ī�޶� ��Ȱ��ȭ
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

    // ������ ���� ó��
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
        //�¿� ĳ���� ȸ��
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
