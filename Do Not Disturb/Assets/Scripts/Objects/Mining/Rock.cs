using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{

    [SerializeField]
    private int hp;                             // 바위의 체력
    [SerializeField]
    private float destroyTime;                  // 깨진 파편 제거 time
    [SerializeField]
    private SphereCollider col;                 // 구체 colider


    // 필요한 게임 오브젝트.
    [SerializeField]
    private GameObject go_rock;                 // 일반 바위
    [SerializeField]
    private GameObject go_debris;               // 깨진 바위
    [SerializeField]
    private GameObject go_effect_prefabs;       // 채굴 이펙트
    [SerializeField]
    public GameObject rock;

    public void Mining()
    {
        var clone = Instantiate(go_effect_prefabs, col.bounds.center, Quaternion.identity);
        Destroy(clone, destroyTime);

        hp--;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Mining);
        if (hp <= 0)
            Destruction();
    }

    // 체력 0 이하면 원본 바위 삭제
    private void Destruction()
    {
        col.enabled = false;
        Destroy(go_rock);
        go_debris.SetActive(true);
        Destroy(go_debris, destroyTime);
        for (int i = 0; i < 5; i++)
        {
            Instantiate(rock, go_debris.transform.GetChild(i).gameObject.transform.position, Quaternion.identity);
        }
        Invoke("CreateDropItem", 0.2f);
    }
}
