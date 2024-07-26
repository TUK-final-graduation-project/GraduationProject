using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHPUIManager : MonoBehaviour
{


    public RectTransform BaseHealthGroup;
    public RectTransform BaseHealthBar;

    bool isTarget = false;


    public void UpdateBaseHP(float baseHP, float MaxHP)
    {
        if ( isTarget)
        {
            BaseHealthBar.transform.localScale = new Vector3(baseHP / MaxHP, 1, 1);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isTarget && other.gameObject.tag == "Player")
        {
            isTarget = true;
            BaseHealthGroup.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isTarget && other.gameObject.tag == "Player")
        {
            isTarget = false;
            BaseHealthGroup.gameObject.SetActive(false);
        }
    }
}
