using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackToBoss : MonoBehaviour
{
    public BossUnit boss;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Sword")
        {
            boss.PlayerDamage(other.transform.position);
        }
    }
}
