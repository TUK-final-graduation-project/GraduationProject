using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserTargetZone : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<EnemyUnitController>() != null)
        {
            collision.gameObject.GetComponent<EnemyUnitController>().OnDestroy();
            // collision.gameObject.GetComponent<UnitCs>().anim.SetBool("isWalk", false);
            // Debug.Log(collision.gameObject.name);
            if (transform.parent.gameObject.GetComponent<UserSpawnPoint>())
            {
                // º´¿µ
                transform.parent.gameObject.GetComponent<UserSpawnPoint>().OnDamage();
            }
            else
            {
                // user home
                transform.parent.gameObject.GetComponent<UserHome>().OnDamage();
            }
        }
    }
}
