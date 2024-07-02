using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class LongRangeUnit : MonoBehaviour
{
    float range = 10f;
    float speed = 5f;
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
            MoveToTarget();
            // FightWithTarget();
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
        // 던지기
        if (curTime <= 0)
        {
            if (!targetUnit.GetComponent<UnitState>().isDead)
            {
                // 상호작용하기 - bullet 스크립트에서 상호작용
                Vector3 dir = (targetUnit.gameObject.transform.position - transform.position).normalized;
                var a = Instantiate(bullet, transform.position, Quaternion.identity);
                a.GetComponent<BulletController>().target = targetUnit.gameObject;
                curTime = maxTime;
            }
        }
        if (targetUnit.GetComponent<UnitState>().isDead)
        {
            // target이 사라졌을 때
            gameObject.GetComponent<UnitMove>().RequestPathToMgr();
            targetUnit = null;
        }
    }

    void MoveToTarget()
    {
        Vector3 dir = (targetUnit.gameObject.transform.position - transform.position).normalized;
        Pos[] p = targetUnit.gameObject.GetComponent<UnitState>().GetNearPosition();
        Debug.Log("여기로 가야지" + p[7].locate);
        transform.position = Vector3.MoveTowards(transform.position, p[7].locate, speed * Time.deltaTime);
    }
}
