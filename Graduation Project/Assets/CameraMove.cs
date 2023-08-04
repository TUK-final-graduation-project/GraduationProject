using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraMove : MonoBehaviour
{
    [SerializeField]
    private float           lookSensitivity;

    [SerializeField]
    private float           cameraRotationLimit;
    private float           currentCameraRotaionX = 0f;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        firstPersonView();
    }

    private void firstPersonView()
    {
        // 상하 회전
        float _xRotaion = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotaion * lookSensitivity;

        currentCameraRotaionX -= _cameraRotationX;
        currentCameraRotaionX = Mathf.Clamp(currentCameraRotaionX, -cameraRotationLimit, cameraRotationLimit);

        transform.localEulerAngles = new Vector3(currentCameraRotaionX, 0f, 0f);   
    }
}
