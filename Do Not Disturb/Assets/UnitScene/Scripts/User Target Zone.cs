using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserTargetZone : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Unit" && collision.gameObject.layer == 12)
        {
            Destroy(collision.gameObject);
            // collision.gameObject.GetComponent<UnitCs>().anim.SetBool("isWalk", false);
            // Debug.Log(collision.gameObject.name);
            if (gameObject.layer == 16)
            {
                transform.parent.gameObject.GetComponent<UserSpawnPoint>().OnDamage();
            }
        }
    }
}
