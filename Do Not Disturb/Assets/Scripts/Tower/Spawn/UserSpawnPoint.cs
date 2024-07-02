using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UserSpawnPoint : MonoBehaviour
{
    public enum Element { fire, earth, ice, tree };
    public enum Type { Melee, Range };

    [Header("spawn point type")]
    public Element element;
    public Type type;

    public GameObject target;

    [Header("target base")]
    public ComSpawnPoint[] spawnPoints;

    [Header("spawn unit type")]
    public GameObject m_EarthUnit;
    public GameObject m_TreeUnit;

    public GameObject r_FireUnit;
    public GameObject r_IceUnit;

    GameObject unit;
    ComSpawnPoint targetPoints;

    public int HP;
    public bool isConquer = false;
    private void Start()
    {
        switch (element)
        {
            case Element.fire:
                unit = r_FireUnit;
                break;
            case Element.earth:
                unit = m_EarthUnit;
                break;
            case Element.ice:
                unit = r_IceUnit;
                break;
            case Element.tree:
                unit = m_TreeUnit;
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

            yield return new WaitForSeconds(2f);
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
