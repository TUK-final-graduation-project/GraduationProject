using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimation : MonoBehaviour
{
    public Unit parent;
    void StartAttack()
    {
        parent.StartAttack();
    }
    void EndAttack()
    {
        parent.EndAttack();
    }
}
