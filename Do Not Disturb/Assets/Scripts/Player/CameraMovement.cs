using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform objectTofollow;    // ī�޶� ����ٴϴ� ���
    public float followSpeed = 10f;     // ī�޶� ����� ���󰡴� �ӵ�
    public float sensitivity = 100f;    // ���콺 ����
    public float clampAngle = 70f;      // ī�޶� ���� ����

    public float rotX;                  // ī�޶��� X�� ȸ�� ����
    public float rotY;                  // ī�޶��� Y�� ȸ�� ����

    public Transform realCamera;        // ������ ���̴� ī�޶� ������Ʈ
    public Vector3 dirNormalized;       // ī�޶� ���ϴ� ������ ����ȭ�� ����
    public Vector3 finalDir;            // ī�޶� ���������� ���ϴ� ����

    public float minDistance;           // ī�޶�� ��� ������ �ּ� �Ÿ�
    public float maxDistance;           // ī�޶�� ��� ������ �ִ� �Ÿ�
    public float finalDistance;         // ī�޶�� ��� ������ ���� �Ÿ�
    public float smoothness = 10f;      // ī�޶� �̵� �� �ε巯�� ����

    // Start is called before the first frame update
    void Start()
    {
       /* // ���󰡴� ������Ʈ�� �ٶ󺸴� ���� ����
        Vector3 lookDirection = objectTofollow.position - transform.position;

        lookDirection.y = 0; // ���� �������θ� �ٶ󺸵��� y���� 0���� ����
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

        // ī�޶��� �ʱ� ȸ���� ���󰡴� ������Ʈ�� �ٶ󺸴� �������� ����
        transform.rotation = targetRotation;

        rotX = 0f;
        rotY = 0f;*/

        rotX = transform.localRotation.eulerAngles.x;
        rotY = transform.localRotation.eulerAngles.y;

        dirNormalized = realCamera.localPosition.normalized;
        finalDistance = realCamera.localPosition.magnitude;

        //// ���콺 Ŀ�� ����
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // ���콺 ȸ�� �� ���ϱ�
        rotX += -(Input.GetAxis("Mouse Y")) * sensitivity * Time.deltaTime;
        rotY += Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle);
        Quaternion rot = Quaternion.Euler(rotX, rotY, 0);
        transform.rotation = rot;
    }

    void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, objectTofollow.position, followSpeed * Time.deltaTime);
        finalDir = transform.TransformPoint(dirNormalized * maxDistance);

        RaycastHit hit;

        if (Physics.Linecast(transform.position, finalDir, out hit))
        {
            finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
        }
        else
        {
            finalDistance = maxDistance;
        }

        realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, dirNormalized * finalDistance, Time.deltaTime * smoothness);
    }
}
