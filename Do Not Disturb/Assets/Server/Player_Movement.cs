using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public float moveSpeed;
    private Rigidbody charRigidbody;
    public Vector3 position;
    public int item;

    void Start()
    {
        charRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        move();

    }

    void move()
    {
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");

        Vector3 inputDir = new Vector3(hAxis, 0, vAxis).normalized;

        charRigidbody.velocity = inputDir * moveSpeed;

        transform.LookAt(transform.position + inputDir);

        Vector3 moveHorizontal = transform.right * hAxis;
        Vector3 moveVertical = transform.forward * vAxis;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * moveSpeed;


        velocity = Vector3.zero;

        charRigidbody.AddForce(velocity * Time.deltaTime, ForceMode.VelocityChange);
        position = charRigidbody.transform.position;

    }

    internal void BroadcastMessage(float moveSpeed)
    {
        throw new NotImplementedException();
    }
}
