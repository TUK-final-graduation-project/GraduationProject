using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public GameObject target;
    bool hasCollided = false;
    float speed = 20f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == target && !hasCollided)
        {
            Debug.Log("!!!!");
            collision.gameObject.GetComponent<UnitState>().UnderAttack(10);
            Destroy(gameObject);
            hasCollided = true;
        }
    }
    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
    }
}
