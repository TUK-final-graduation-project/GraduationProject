using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTowerController : MonoBehaviour
{
    [Header("UNIT")]
    [SerializeField] GameObject Minion;
    [SerializeField] Transform StartPosition;
    [SerializeField] float minionSpeed;
    [SerializeField] float CurTime;
    [SerializeField] float MaxTime;

    public EnemyBaseController[] enemyBase;
    Vector3 targetBase;

    int minion_count = 0;

    void Start()
    {
        CurTime = MaxTime;
        float minDistance = float.MaxValue;
        float tmp = 0f;
        enemyBase = FindObjectsOfType(typeof(EnemyBaseController)) as EnemyBaseController[];
        foreach (EnemyBaseController b in enemyBase)
        {
            tmp = Vector3.Distance(transform.position, b.transform.position);
            if (minDistance > tmp)
            {
                minDistance = tmp;
                targetBase = b.transform.position;
            }
        }
    }

    void Update()
    {
        CurTime -= Time.deltaTime;
        if (CurTime <= 0 && minion_count == 0)
        {
            var a = Instantiate(Minion, StartPosition.position, StartPosition.rotation);
            a.name = minion_count.ToString();
            a.GetComponent<UnitMove>().target = targetBase;
            CurTime = MaxTime;
            minion_count++;
        }
    }
}
