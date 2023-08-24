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
        //�¿� ĳ���� ȸ��
        float yRotaion = Input.GetAxisRaw("Mouse X");
        Vector3 characterRotationY = new Vector3(0f, yRotaion, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(characterRotationY));

    }
}
