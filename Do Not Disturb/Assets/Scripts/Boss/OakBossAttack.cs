using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OakBossAttack : MonoBehaviour
{
    float time;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Unit")
        {
            other.GetComponent<UnitCs>().HitByOakBoss();
        }
    }

    private void Update()
    {
        time += 0.1f;
        if ( time >= 3f)
        {
            Destroy(gameObject);
        }
    }
}
