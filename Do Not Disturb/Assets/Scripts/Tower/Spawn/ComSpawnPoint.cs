using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComSpawnPoint : MonoBehaviour
{
    public enum Element { fire, earth, ice, tree };
    public enum Type { Melee, Range };

    [Header("spawn point type")]
    public Element element;
    public int HP;
    public bool isConquer = false;

    public GameObject UserBase;
    public GameObject target;

    [Header("spawn unit type")]
    public GameObject Golem;
    public GameObject Dear;
    public GameObject Cannon;
    public GameObject Snowman;

    public Tower[] towers;
    
    GameObject unit;

    private void Awake()
    {
        switch (element)
        {
            case Element.fire:
                unit = Cannon;
                break;
            case Element.earth:
                unit = Golem;
                break;
            case Element.ice:
                unit = Snowman;
                break;
            case Element.tree:
                unit = Dear;
                break;
        }

    }

    public IEnumerator Spawn(int num)
    {
        for (int i = 0; i < num; i++)
        {
            Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            GameObject _iUnit = Instantiate(unit, transform.position, targetRotation);
            _iUnit.GetComponent<UnitCs>().target = target;
            _iUnit.GetComponent<UnitCs>().UserBase = UserBase;

            yield return new WaitForSeconds(4f);
        }
        yield return new WaitForSeconds(0.1f);
    }

    public void StartSpawn(int num)
    {
        float minDistance = float.MaxValue;
        float tmp = 0;
        target = UserBase;
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
