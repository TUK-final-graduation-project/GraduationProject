using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TargetZone : MonoBehaviour
{
    public ComSpawnPoint[] spawnPoints;


    private void Awake()
    {
        spawnPoints = FindObjectsOfType(typeof(ComSpawnPoint)) as ComSpawnPoint[];
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Unit" && collision.gameObject.layer == 10 )
        {
            if ( transform.parent.gameObject.GetComponent<ComSpawnPoint>().isConquer)
            {
                float minDistance = float.MaxValue;
                float tmp;
                foreach (ComSpawnPoint b in spawnPoints)
                {
                    if (!b.isConquer)
                    {
                        tmp = Vector3.Distance(transform.position, b.transform.position);
                        if (minDistance > tmp)
                        {
                            minDistance = tmp;
                            collision.gameObject.GetComponent<UnitCs>().target = b.gameObject;
                            collision.gameObject.GetComponent<UnitCs>().RequestPathToMgr();
                        }
                    }
                }
                if (collision.gameObject.GetComponent<UnitCs>().target == gameObject)
                {
                    // game end
                }
            }
            else
            {

                collision.gameObject.GetComponent<UnitCs>().OnDestroy();
                // collision.gameObject.GetComponent<UnitCs>().anim.SetBool("isWalk", false);
                // Debug.Log(collision.gameObject.name);
                if ( gameObject.layer == 17)
                {
                    transform.parent.gameObject.GetComponent<ComSpawnPoint>().OnDamage();
                }
            }
        }
    }
}
