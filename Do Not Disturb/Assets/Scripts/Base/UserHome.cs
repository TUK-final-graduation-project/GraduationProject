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
            // ���� ����
            // userbase�� �����Ƿ� ��~!
        }
    }

    public void setHP(int value)
    {
        HP = value;
    }
}
