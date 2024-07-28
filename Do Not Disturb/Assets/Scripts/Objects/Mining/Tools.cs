using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tools : MonoBehaviour
{
    [SerializeField]
    public float rateTime = 0.4f;
    [SerializeField]
    public float onTime = 0.5f;
    [SerializeField]
    public float endRateTime = 0.3f;
    public BoxCollider area;
    public GameObject effect;

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
        yield return new WaitForSeconds(rateTime); // ���� �ð�

        area.enabled = true;
        effect.SetActive(true);

        // �ۿ��ϴ� �ð�
        yield return new WaitForSeconds(onTime);    // �浹 ���� �� ȿ�� ���� �ð�

        area.enabled = false;
        effect.SetActive(false);

        yield return new WaitForSeconds(endRateTime);   // ���� ��� �� ���� �ð�

    }
}
