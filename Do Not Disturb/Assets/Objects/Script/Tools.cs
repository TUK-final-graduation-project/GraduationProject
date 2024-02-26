using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools : MonoBehaviour
{
    public float rate;
    public BoxCollider area;

    private void Start()
    {
        // 필요한 컴포넌트를 초기화
        //player = GetComponent<PlayerMove>();
        //rock = GetComponent<Rock>();
    }

    public void Use()
    {
        StopCoroutine("Swing");
        StartCoroutine("Swing");
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
        area.enabled = true;

        yield return new WaitForSeconds(0.3f);
        area.enabled = false;

        yield return new WaitForSeconds(0.3f);

    }
}
