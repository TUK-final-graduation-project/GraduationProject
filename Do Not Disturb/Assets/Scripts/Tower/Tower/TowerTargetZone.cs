using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTargetZone : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<EnemyUnitController>() != null)
        {
            collision.gameObject.GetComponent<EnemyUnitController>().OnDestroy();
            transform.parent.gameObject.GetComponent<Tower>().OnDamage();

            if (transform.parent.gameObject.GetComponent<Tower>().isConquer)
            {
                collision.gameObject.GetComponent<EnemyUnitController>().target = 
                    collision.gameObject.GetComponent<EnemyUnitController>().Base;
                collision.gameObject.GetComponent<EnemyUnitController>().RequestPathToMgr();
            }
        }
    }
}
