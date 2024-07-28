using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UserHome : MonoBehaviour
{
    public float baseHP;
    public float MaxHP;

    public RectTransform BaseHealthGroup;
    public RectTransform BaseHealthBar;

    private void Update()
    {
        if (BaseHealthGroup != null)
        {
            BaseHealthBar.transform.localScale = new Vector3(baseHP / MaxHP, 1, 1);
        }
    }
    public void OnDamage()
    {
        baseHP -= 50;
        if (baseHP < 0)
        {
            SceneManager.LoadScene(6);
        }
    }

    public void setHP(int value)
    {
        baseHP = value;
    }
}
