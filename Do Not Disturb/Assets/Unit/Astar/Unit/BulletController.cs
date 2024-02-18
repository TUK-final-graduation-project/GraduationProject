using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public GameObject target;
    int i = 0;
    bool hasCollided = false;
    public GameObject parentUnit = null;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == target && !hasCollided)
        {
            collision.gameObject.GetComponent<UnitState>().UnderAttack(10);
            Debug.Log(i);
            Destroy(gameObject);
            hasCollided = true;
        }
    }
}
