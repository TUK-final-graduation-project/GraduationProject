using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OakBossAttack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Unit")
        {
            other.GetComponent<UnitCs>().HitByBoss();
        }
    }
}
