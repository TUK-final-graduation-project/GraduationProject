using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseTargetZone : MonoBehaviour
{
    public ComSpawnPoint[] spawnPoints;
    private void Awake()
    {
        spawnPoints = FindObjectsOfType(typeof(ComSpawnPoint)) as ComSpawnPoint[];
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<OurUnitController>() != null)
        {
            if (transform.parent.gameObject.GetComponent<ComSpawnPoint>().isConquer)
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
                            collision.gameObject.GetComponent<OurUnitController>().target = b.gameObject;
                            collision.gameObject.GetComponent<OurUnitController>().Base = b.gameObject;
                            collision.gameObject.GetComponent<OurUnitController>().RequestPathToMgr();
                        }
                    }
                }
                if (collision.gameObject.GetComponent<OurUnitController>().target == gameObject)
                {
                    // game end
                }
            }
            else
            {
                collision.gameObject.GetComponent<OurUnitController>().OnDestroy();
                transform.parent.gameObject.GetComponent<ComSpawnPoint>().OnDamage();
            }
        }
    }
}
