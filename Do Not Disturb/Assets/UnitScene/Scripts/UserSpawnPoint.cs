using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class UserSpawnPoint : MonoBehaviour
{
    public enum Element { fire, earth, ice, tree };
    public enum Type { Melee, Range };

    [Header("spawn point type")]
    public Element element;
    public Type type;

    public GameObject target;

    [Header("spawn unit type")]
    public GameObject m_FireUnit;
    public GameObject m_EarthUnit;
    public GameObject m_IceUnit;
    public GameObject m_TreeUnit;

    public GameObject r_FireUnit;
    public GameObject r_EarthUnit;
    public GameObject r_IceUnit;
    public GameObject r_TreeUnit;

    GameObject unit;

    private void Start()
    {
        switch (element)
        {
            case Element.fire:
                unit = type == Type.Melee ? m_FireUnit : r_FireUnit;
                break;
            case Element.earth:
                unit = type == Type.Melee ? m_EarthUnit : r_EarthUnit;
                break;
            case Element.ice:
                unit = type == Type.Melee ? m_IceUnit : r_IceUnit;
                break;
            case Element.tree:
                unit = type == Type.Melee ? m_TreeUnit : r_TreeUnit;
                break;

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
}
