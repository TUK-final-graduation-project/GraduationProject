using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttack : MonoBehaviour
{
    public int damage;
    public bool isMelee;

    float time;

    private void Update()
    {
        // �ڵ� ����
        time += 1f;

        if (time > 1000f && !isMelee)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if( !isMelee && other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
        else if ( !isMelee && other.gameObject.tag == "Ground")
        {
            Destroy(gameObject);
        }
    }

    ///////////// set �Լ��� /////////////////
    public void SetDamage(int _damage)
    {
        damage = _damage;
    }
    /////////////////////////////////////////
}
