using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserHome : MonoBehaviour
{
    public int HP;

    public void OnDamage()
    {
        HP -= 50;
        if ( HP < 0)
        {
            // 승패 판정
            // userbase가 졌으므로 끝~!
        }
    }

    public void setHP(int value)
    {
        HP = value;
    }
}
