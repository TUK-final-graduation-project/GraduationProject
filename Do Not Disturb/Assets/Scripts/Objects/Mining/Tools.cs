using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools : MonoBehaviour
{
    public float rate;
    public BoxCollider area;

    private void Start()
    {

    }

    public void Use()
    {
        StopCoroutine("Swing");
        StartCoroutine("Swing");
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.4f);
        area.enabled = true;

        // Į���� �ۿ��ϴ� �ð�
        yield return new WaitForSeconds(0.5f);
        area.enabled = false;

        yield return new WaitForSeconds(0.3f);

    }
}
