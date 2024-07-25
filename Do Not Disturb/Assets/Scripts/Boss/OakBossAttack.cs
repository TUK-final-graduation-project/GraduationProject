using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OakBossAttack : MonoBehaviour
{
    float time;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Unit" && other.GetComponent<OurUnitController>() != null)
        {
            other.GetComponent<OurUnitController>().OnHitEnter("Oak", transform.position);
        }
    }

    private void Update()
    {
        time += 0.1f;
        if ( time >= 3f)
        {
            Destroy(gameObject, 4);
        }
    }
}
