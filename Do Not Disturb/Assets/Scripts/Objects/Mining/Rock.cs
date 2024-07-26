using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour
{

    [SerializeField]
    private int hp;                             // ������ ü��
    [SerializeField]
    private float destroyTime;                  // ���� ���� ���� time
    [SerializeField]
    private SphereCollider col;                 // ��ü colider


    // �ʿ��� ���� ������Ʈ.
    [SerializeField]
    private GameObject go_rock;                 // �Ϲ� ����
    [SerializeField]
    private GameObject go_debris;               // ���� ����
    [SerializeField]
    private GameObject go_effect_prefabs;       // ä�� ����Ʈ
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

    // ü�� 0 ���ϸ� ���� ���� ����
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
