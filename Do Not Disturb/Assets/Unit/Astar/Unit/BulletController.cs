using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public GameObject target;
    int i = 0;
    bool hasCollided = false;
    public GameObject parentUnit = null;

    private void OnTriggetEnter(Collision collision)
    {
        if (collision.gameObject == target && !hasCollided)
        {
            Debug.Log("!!!!");
            collision.gameObject.GetComponent<UnitState>().UnderAttack(10);
            Debug.Log(i);
            Destroy(gameObject);
            hasCollided = true;
        }
    }
    private void Update()
    {
        if (!target.GetComponent<UnitState>().isDead)
        {
            Vector3 dir = (target.transform.position - transform.position).normalized;
            transform.position += dir * Time.deltaTime * 10f;
            // transform.position = new Vector3(transform.position.x, 1f, transform.position.z);
        }
    }
}
