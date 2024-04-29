using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMove : MonoBehaviour
{
    public float speed = 5f;
    private Rigidbody characterRigidbody;

    void Start()
    {
        characterRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");
        // -1 ~ 1

        float fallSpeed = characterRigidbody.velocity.y; // 떨어지는 속도 저장

        Vector3 velocity = new Vector3(inputX, 0, inputZ);
        velocity *= speed;
        velocity.y = fallSpeed; // 떨어지는 속도 초기화
        characterRigidbody.velocity = velocity;
    }
}

