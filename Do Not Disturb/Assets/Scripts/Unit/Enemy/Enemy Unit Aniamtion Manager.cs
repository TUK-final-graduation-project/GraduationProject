using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnitAniamtionManager : MonoBehaviour
{
    public EnemyUnitController parent;
    void StartAttack()
    {
        parent.StartAttack();
    }
    void EndAttack()
    {
        parent.EndAttack();
    }
}
    