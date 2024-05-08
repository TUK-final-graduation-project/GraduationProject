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
            if (transform.parent.gameObject.GetComponent<Tower>().isConquer)
            {
                // �ٸ� Ÿ�� �˷��ֱ� �Ƹ��� user base�� �� �� ������ �̰� �� ��Ƴ�
                //collision.gameObject.GetComponent<UnitCs>().target = b.gameObject;
                //float minDistance = float.MaxValue;
                //float tmp;
                //foreach (ComSpawnPoint b in spawnPoints)
                //{
                //    if (!b.isConquer)
                //    {
                //        tmp = Vector3.Distance(transform.position, b.transform.position);
                //        if (minDistance > tmp)
                //        {
                //            minDistance = tmp;
                //            collision.gameObject.GetComponent<UnitCs>().target = b.gameObject;
                //            collision.gameObject.GetComponent<UnitCs>().RequestPathToMgr();
                //        }
                //    }
                //}
            }
            else
            {
                collision.gameObject.GetComponent<UnitCs>().OnDestroy();
                // collision.gameObject.GetComponent<UnitCs>().anim.SetBool("isWalk", false);
                // Debug.Log(collision.gameObject.name);
                transform.parent.gameObject.GetComponent<Tower>().OnDamage();
            }
        }
    }
}
