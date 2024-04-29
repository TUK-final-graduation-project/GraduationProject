using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnotherPlayer : MonoBehaviour
{
    Client client;
    // 플레이어의 이동 속도
    public float moveSpeed = 5f;
    public Vector3 position;
    private Rigidbody charRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        client = FindObjectOfType<Client>();
        charRigidbody = GetComponent<Rigidbody>();
        position = new Vector3(10, 100, 100);
    }

    // Update is called once per frame
    void Update()
    {
        // 서버로부터 데이터를 받아서 위치 업데이트
        position = client.getPlayerPosition();
        transform.position = position;
    }


}
