using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTargetZone : MonoBehaviour
{
    public ComSpawnPoint[] spawnPoints;

    private void Awake()
    {
        spawnPoints = FindObjectsOfType(typeof(ComSpawnPoint)) as ComSpawnPoint[];
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Unit")
        {
            // 데모를 위한 block
            //collision.gameObject.GetComponent<UnitCs>().OnDestroy();

            // collision.gameObject.GetComponent<UnitCs>().anim.SetBool("isWalk", false);
            // Debug.Log(collision.gameObject.name);
            transform.parent.gameObject.GetComponent<Tower>().OnDamage();
        }
    }
}
