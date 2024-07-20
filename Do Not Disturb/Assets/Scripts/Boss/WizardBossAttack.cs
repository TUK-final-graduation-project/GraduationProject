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

    bool isRight = false;

    private void Start()
    {
        isExplosion = false;
        dir = FinalPosition - transform.position;
        dir = dir.normalized;

        if ( FinalPosition.x >= transform.position.x)
        {
            isRight = true;
        }
    }

    private void Update()
    {
        if (isRight && transform.position.x <= FinalPosition.x)
        {
            transform.position += dir * 0.7f;
        }
        else if ( !isRight && transform.position.x >= FinalPosition.x)
        {
            transform.position += dir * 0.7f;
        }
        else
        {

            isExplosion = true;

            Destroy(gameObject, 1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ( other.tag == "Unit" )
        {
            other.GetComponent<UnitCs>().HitByWizardBoss();
        }
    }
}
