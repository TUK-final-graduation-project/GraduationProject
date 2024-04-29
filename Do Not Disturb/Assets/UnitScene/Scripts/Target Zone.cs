using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetZone : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Unit")
        {
            collision.gameObject.GetComponent<UnitCs>().anim.SetBool("isWalk", false);
            // Debug.Log(collision.gameObject.name);
        }
    }
}
