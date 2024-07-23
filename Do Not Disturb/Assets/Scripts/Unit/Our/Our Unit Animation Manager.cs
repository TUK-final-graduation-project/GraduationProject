using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OurUnitAnimationManager : MonoBehaviour
{
    public OurUnitController parent;
    void StartAttack()
    {
        parent.StartAttack();
    }
    void EndAttack()
    {
        parent.EndAttack();
    }
}
