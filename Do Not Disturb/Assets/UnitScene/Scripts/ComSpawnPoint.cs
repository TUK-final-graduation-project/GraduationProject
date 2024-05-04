using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComSpawnPoint : MonoBehaviour
{
    public enum Element { fire, earth, ice, tree };
    public enum Type { Melee, Range };

    [Header("spawn point type")]
    public Element element;
    public Type type;
    public int HP;
    public bool isConquer = false;

    public GameObject comTarget;
    public GameObject target;

    [Header("spawn unit type")]
    public GameObject m_EarthUnit;
    public GameObject m_TreeUnit;

    public GameObject r_FireUnit;
    public GameObject r_IceUnit;

    public Tower[] towers;
    
    GameObject unit;

    private void Awake()
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

        Debug.Log(unit.gameObject.name);
    }

    public IEnumerator Spawn(int num)
    {
        for (int i = 0; i < num; i++)
        {
            Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            GameObject _iUnit = Instantiate(unit, transform.position, targetRotation);
            _iUnit.GetComponent<UnitCs>().target = target;
            _iUnit.GetComponent<UnitCs>().ComTarget = comTarget;

            yield return new WaitForSeconds(2f);
        }
        yield return new WaitForSeconds(0.1f);
    }

    public void StartSpawn(int num)
    {
        float minDistance = float.MaxValue;
        float tmp = 0;
        towers = FindObjectsOfType(typeof(Tower)) as Tower[];
        foreach (Tower tower in towers)
        {
            tmp = Vector3.Distance(transform.position, tower.transform.position);
            if (minDistance > tmp)
            {
                minDistance = tmp;
                target = tower.gameObject;
            }
        }
        StopCoroutine(Spawn(num));
        StartCoroutine(Spawn(num));
    }
    public void OnDamage()
    {
        HP -= 50;
        if ( HP < 0)
        {
            isConquer = true;
        }
    }
}
