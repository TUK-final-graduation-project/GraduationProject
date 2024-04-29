using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnotherPlayer : MonoBehaviour
{
    Client client;
    // �÷��̾��� �̵� �ӵ�
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
        // �����κ��� �����͸� �޾Ƽ� ��ġ ������Ʈ
        position = client.getPlayerPosition();
        transform.position = position;
    }


}
