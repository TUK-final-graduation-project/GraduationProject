using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class WizardBossAttack : MonoBehaviour
{
    public Rigidbody rigid;

    public bool isExplosion;

    public Vector3 FinalPosition;

    Vector3 dir = new Vector3(-1f, 0, 0);

    private void Start()
    {
        isExplosion = false;
        dir = FinalPosition - transform.position;
        dir = dir.normalized;
    }

    private void Update()
    {
        if (transform.position.x <= FinalPosition.x)
        {
            transform.position += dir * 0.7f;
        }
        else
        {

            isExplosion = true;

            Destroy(gameObject, 1);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if ( other.tag == "User")
        {
            other.GetComponent<UnitCs>().HitByBoss();
        }
    }
}
