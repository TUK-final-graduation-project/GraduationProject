using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public enum Element { fire, earth, ice, tree };
    public enum Team { com, user };
    public enum Type { Melee, Range };

    [Header("spawn point type")]
    public Element element;
    public Type type;

    public GameObject fireUnit;
    public GameObject earthUnit;
    public GameObject iceUnit;
    public GameObject treeUnit;

    GameObject unit;

    private void Awake()
    {
        switch (element)
        {
            case Element.fire:
                unit = fireUnit;
                unit.GetComponent<Unit>().type = type == Type.Melee ? Unit.Type.Melee : Unit.Type.Range;
                break;
            case Element.earth:
                unit = earthUnit;
                unit.GetComponent<Unit>().type = type == Type.Melee ? Unit.Type.Melee : Unit.Type.Range;
                break;
            case Element.ice:
                unit = iceUnit;
                unit.GetComponent<Unit>().type = type == Type.Melee ? Unit.Type.Melee : Unit.Type.Range;
                break;
            case Element.tree:
                unit = treeUnit;
                unit.GetComponent<Unit>().type = type == Type.Melee ? Unit.Type.Melee : Unit.Type.Range;
                break;
        }

        Debug.Log(unit.gameObject.GetComponent<Unit>().type);
    }
}
