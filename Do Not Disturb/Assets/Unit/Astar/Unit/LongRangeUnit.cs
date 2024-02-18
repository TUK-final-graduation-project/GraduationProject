using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
            }
        }
    }

    void FightWithTarget()
    {
        // ������
        if (curTime <= 0)
        {
            if (!targetUnit.GetComponent<UnitState>().isDead)
            {
                // ��ȣ�ۿ��ϱ� - bullet ��ũ��Ʈ���� ��ȣ�ۿ�
                Vector3 dir = (targetUnit.gameObject.transform.position - transform.position).normalized;
                var a = Instantiate(bullet, transform.position, transform.rotation);
                a.GetComponent<Rigidbody>().AddForce(dir * bulletSpeed, ForceMode.Impulse);
                a.GetComponent<BulletController>().target = targetUnit.gameObject;
                curTime = maxTime;
            }
        }
        if (targetUnit.GetComponent<UnitState>().isDead)
        {
            Debug.Log("Dead!");
            // target�� ������� ��
            // 1. ������ ��
            // 2. �ٽ� ��� Ž���ϱ�
            gameObject.GetComponent<UnitMove>().RequestPathToMgr();
            // 3. target == null
            targetUnit = null;
        }
    }
}
