using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongRangeUnit : MonoBehaviour
{
    float range = 10f;

    [Header("Material")]
    public Material detectedMat;

    [Header("Target")]
    public Collider targetUnit = null;
    public Collider[] colliders;

    [Header("Fight")]
    public GameObject bullet;
    [SerializeField] float maxTime = 3f;
    [SerializeField] float curTime = 1f;
    public float bulletSpeed = 30f;

    void changeMaterial(GameObject go, Material changeMat)
    {
        Renderer rd = go.GetComponent<MeshRenderer>();
        Material[] mat = rd.sharedMaterials;
        mat[0] = changeMat;
        rd.materials = mat;
    }

    void Update()
    {
        if ( targetUnit == null )
        {
            SearchNearUnit();
        }
        else
        {
            curTime -= Time.deltaTime;
            FightWithTarget();
        }
    }

    void SearchNearUnit()
    {
        colliders = Physics.OverlapSphere(transform.position, range);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != gameObject && collider.gameObject.tag == "Fire")
            {
                targetUnit = collider;
                changeMaterial(collider.gameObject, detectedMat);
                gameObject.GetComponent<UnitMove>().StopCoroutine("FollowPath");
                Debug.Log(gameObject.name);
            }
        }
    }

    void FightWithTarget()
    {
        // ������
        if (curTime <= 0)
        {
            Vector3 dir = (targetUnit.gameObject.transform.position - transform.position).normalized;
            var a = Instantiate(bullet, transform.position, transform.rotation);
            a.GetComponent<Rigidbody>().AddForce(dir * bulletSpeed, ForceMode.Impulse);
            curTime = maxTime;
        }

        // ��ȣ�ۿ��ϱ� - bullet ��ũ��Ʈ���� ��ȣ�ۿ�

        // target�� ������� ��
        // 1. ������ ��
        // 2. �ٽ� ��� Ž���ϱ�
        // 3. target == null
    }
}
