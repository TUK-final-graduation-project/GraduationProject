using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform objectTofollow;    // 카메라가 따라다니는 대상
    public float followSpeed = 10f;     // 카메라가 대상을 따라가는 속도
    public float sensitivity = 100f;    // 마우스 감도
    public float clampAngle = 70f;      // 카메라 각도 제한

    public float rotX;                  // 카메라의 X축 회전 각도
    public float rotY;                  // 카메라의 Y축 회전 각도

    public Transform realCamera;        // 실제로 보이는 카메라 오브젝트
    public Vector3 dirNormalized;       // 카메라가 향하는 방향의 정규화된 벡터
    public Vector3 finalDir;            // 카메라가 최종적으로 향하는 방향

    public float minDistance;           // 카메라와 대상 사이의 최소 거리
    public float maxDistance;           // 카메라와 대상 사이의 최대 거리
    public float finalDistance;         // 카메라와 대상 사이의 최종 거리
    public float smoothness = 10f;      // 카메라 이동 시 부드러운 정도

    // Start is called before the first frame update
    void Start()
    {
       /* // 따라가는 오브젝트를 바라보는 방향 벡터
        Vector3 lookDirection = objectTofollow.position - transform.position;

        lookDirection.y = 0; // 수평 방향으로만 바라보도록 y값을 0으로 설정
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

        // 카메라의 초기 회전을 따라가는 오브젝트를 바라보는 방향으로 설정
        transform.rotation = targetRotation;

        rotX = 0f;
        rotY = 0f;*/

        rotX = transform.localRotation.eulerAngles.x;
        rotY = transform.localRotation.eulerAngles.y;

        dirNormalized = realCamera.localPosition.normalized;
        finalDistance = realCamera.localPosition.magnitude;

        //// 마우스 커서 삭제
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // 마우스 회전 값 구하기
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
