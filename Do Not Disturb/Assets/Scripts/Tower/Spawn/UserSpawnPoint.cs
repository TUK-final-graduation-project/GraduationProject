using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UserSpawnPoint : MonoBehaviour
{
    public enum Type { Melee, Range };

    [Header("spawn point type")]
    public Type type;
    public GameObject Base;

    public GameObject target;

    [Header("target base")]
    public ComSpawnPoint[] spawnPoints;

    [Header("spawn unit type")]
    public GameObject shooter;
    public GameObject swordsman;

    GameObject unit;
    ComSpawnPoint targetPoints;

    public int HP;
    public bool isConquer = false;

    int num = 10;


    private void Start()
    {
        switch (type)
        {
            case Type.Melee:
                unit = swordsman;
                break;
            case Type.Range:
                unit = shooter;
                break;

        }

        float minDistance = float.MaxValue;

        spawnPoints = FindObjectsOfType(typeof(ComSpawnPoint)) as ComSpawnPoint[];
        float tmp = 0;
        foreach (ComSpawnPoint b in spawnPoints)
        {
            if (!b.isConquer)
            {
                tmp = Vector3.Distance(transform.position, b.transform.position);
                if (minDistance > tmp)
                {
                    minDistance = tmp;
                    target = b.gameObject;
                }
            }
        }
        //unit.GetComponent<UnitCs>().target = target;
        //GameObject _iUnit = Instantiate(unit, transform.position, transform.rotation);
        //_iUnit.GetComponent<UnitCs>().RequestPathToMgr();
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        while(true)
        {
            Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            GameObject _iUnit = Instantiate(unit, transform.position, targetRotation);
            _iUnit.GetComponent<UnitCs>().target = target;

            // _iUnit.GetComponent<UnitCs>().RequestPathToMgr();

            num -= 1;

            if ( num <= 0)
            {
                yield break;
            }
            yield return new WaitForSeconds(4f);
        }
    }

    private void Update()
    {
        if ( num <= 0)
        {
            StopCoroutine("Spawn");
            Destroy(Base);
        }
    }
    public void OnDamage()
    {
        HP -= 50;
        if (HP < 0)
        {
            isConquer = true;
        }
    }
}
