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
        yield return new WaitForSeconds(rateTime); // 지연 시간

        area.enabled = true;
        effect.SetActive(true);

        // 작용하는 시간
        yield return new WaitForSeconds(onTime);    // 충돌 영역 및 효과 지속 시간

        area.enabled = false;
        effect.SetActive(false);

        yield return new WaitForSeconds(endRateTime);   // 도구 사용 후 지연 시간

    }
}
