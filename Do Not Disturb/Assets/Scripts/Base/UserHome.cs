using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserHome : MonoBehaviour
{
    public int HP;

    public void OnDamage()
    {
        HP -= 50;
        if ( HP < 0)
        {
            SceneManager.LoadScene(6);
        }
    }

    public void setHP(int value)
    {
        HP = value;
    }
}
