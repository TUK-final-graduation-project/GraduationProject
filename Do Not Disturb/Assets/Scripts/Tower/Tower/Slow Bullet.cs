using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowBullet : MonoBehaviour
{

    public Vector3 TargetPosition;
    Vector3 dir;
    bool isArrive;
    private void Start()
    {
        dir = TargetPosition - transform.position;
        dir = dir.normalized;
    }
    private void Update()
    {
        if (!isArrive)
        {
            if (transform.position.y >= 0)
            {
                transform.position += dir;
            }
            else
            {
                isArrive = true;
                MakeSlow();
            }
        }
        else
        {
            Destroy(gameObject, 3);
        }
    }

    void MakeSlow()
    {

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0f, LayerMask.GetMask("Com"));

        foreach (RaycastHit hitObj in rayHits)
        {
            if (hitObj.transform.GetComponent<EnemyUnitController>() != null)
            {
                Debug.Log("dd");
                hitObj.transform.GetComponent<EnemyUnitController>().OnHitEnter("Slow", transform.position);
                break;
            }
        }
        Destroy(gameObject);

    }
}
