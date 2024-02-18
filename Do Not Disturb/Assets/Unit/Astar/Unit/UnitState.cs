using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitState : MonoBehaviour
{
    public int HP = 10000000;
    public bool isDead = false;
    public void UnderAttack(int damage)
    {
        if(HP > 0)
        {
            HP -= damage;
        }
        else
        {
            isDead = true;
        }
    }
    private void Update()
    {
        if (isDead)
        {
            Destroy(gameObject);
        }
    }
}
