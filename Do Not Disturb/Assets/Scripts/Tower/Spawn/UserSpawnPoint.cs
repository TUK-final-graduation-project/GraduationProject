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

    int num = 5;


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
        Base = spawnPoints[0].gameObject;
        foreach (ComSpawnPoint b in spawnPoints)
        {
            if (!b.isConquer)
            {
                tmp = Vector3.Distance(transform.position, b.transform.position);
                if (minDistance > tmp)
                {
                    minDistance = tmp;
                    Base = b.gameObject;
                }
            }
        }
        target = Base;
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        while(true)
        {
            Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            GameObject _iUnit = Instantiate(unit, transform.position, targetRotation);
            _iUnit.GetComponent<OurUnitController>().target = target;
            _iUnit.GetComponent<OurUnitController>().Base = Base;

            num -= 1;

            if ( num <= 0)
            {
                Destroy(transform.parent.gameObject );
                yield break;
            }
            yield return new WaitForSeconds(7f);
        }
    }

    private void Update()
    {
        if ( num <= 0)
        {
            StopCoroutine(Spawn());
            Destroy(gameObject);
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
