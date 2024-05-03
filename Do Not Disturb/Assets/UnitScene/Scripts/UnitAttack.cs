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
        time += 1f;

        if (time > 100f && !isMelee)
        {
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ground")
        {
            Destroy(gameObject, 3);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if( !isMelee && other.gameObject.tag == "Wall")
        {
            Destroy(gameObject);
        }
    }
}
