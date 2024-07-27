using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComSpawnPoint : MonoBehaviour
{
    public enum Element { fire, earth, ice, tree };
    public enum Type { Melee, Range };

    [Header("spawn point type")]
    public Element element;
    public float HP = 600f;
    public float MaxHP = 600f;
    public bool isConquer = false;
    public List<int> SpawnUnitNum = new List<int>();

    public GameObject UserBase;
    public GameObject target;

    [Header("spawn unit type")]
    public GameObject Golem;
    public GameObject Dear;
    public GameObject Cannon;
    public GameObject Snowman;

    public Tower[] towers;

    public BaseHPUIManager UpdateHP;

    GameObject unit;

    private void Awake()
    {
        HP = MaxHP;
        target = UserBase;
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
    private void Update()
    {
        if (UpdateHP.isTarget)
        {
            UpdateHP.UpdateBaseHP(HP, MaxHP);
        }
    }
    public IEnumerator Spawn(int num)
    {
        for (int i = 0; i < num; i++)
        {
            Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
            GameObject _iUnit = Instantiate(unit, transform.position, targetRotation);
            _iUnit.GetComponent<EnemyUnitController>().target = target;
            _iUnit.GetComponent<EnemyUnitController>().Base = UserBase;

            yield return new WaitForSeconds(5f);
        }
        yield return new WaitForSeconds(0.1f);
    }

    public void StartSpawn(int num)
    {
        float minDistance = float.MaxValue;
        float tmp;
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
