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

    Animator _animator;
    Camera _camera;
    CharacterController _controllor;

    public bool toggleCameraRotation;
    public bool run;

    // Start is called before the first frame update
    void Start()
    {
        _animator = this.GetComponent<Animator>();
        _camera = Camera.main;
        _controllor = this.GetComponent<CharacterController>();     
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftAlt))
        {
            toggleCameraRotation = true;   
        } else { 
            toggleCameraRotation = false; 
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            run = true;
        }
        else
        {
            run = false;
        }
        InputMovement();
    }

    void LateUpdate()
    {
        if(toggleCameraRotation!=true)
        {
            Vector3 playerRotate = Vector3.Scale(_camera.transform.forward, new Vector3(1,0,1));
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerRotate), Time.deltaTime * smoothness);
        }    
    }

    void InputMovement()
    {
        finalSpeed = (run) ? runSpeed : speed; 
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        Vector3 moveDirection = forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal");

        _controllor.Move(moveDirection.normalized*finalSpeed*Time.deltaTime);
        float percent = ((run) ? 1: 0.5f) * moveDirection.magnitude;
        _animator.SetFloat("Blend", percent, 0.5f, Time.deltaTime);
    }
}
