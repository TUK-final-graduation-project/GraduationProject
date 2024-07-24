using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTargetZone : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<EnemyUnitController>() != null)
        {
            other.gameObject.GetComponent<EnemyUnitController>().OnDestroy();
            transform.parent.gameObject.GetComponent<Tower>().OnDamage();

            if (transform.parent.gameObject.GetComponent<Tower>().isConquer)
            {
                other.gameObject.GetComponent<EnemyUnitController>().target =
                    other.gameObject.GetComponent<EnemyUnitController>().Base;
                other.gameObject.GetComponent<EnemyUnitController>().RequestPathToMgr();
            }
        }
    }

}
