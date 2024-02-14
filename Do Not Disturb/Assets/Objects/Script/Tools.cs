using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools : MonoBehaviour
{
    public int damage;
    public float rate;
    public BoxCollider area;

    public void Use()
    {
        StopCoroutine("Swing");
        StartCoroutine("Swing");
    }

    IEnumerator Swing()
    {
        yield return new WaitForSeconds(0.1f);
        area.enabled = true;
        //trailEffect.enableed = true;

        yield return new WaitForSeconds(0.3f);
        area.enabled = false;

        yield return new WaitForSeconds(0.3f);
        //trailEffect.enabled = false;

    }
}
