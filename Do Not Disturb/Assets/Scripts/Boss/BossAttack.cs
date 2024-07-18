using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttack : MonoBehaviour
{
    public GameObject meshObj;
    public GameObject effectObj;
    public Rigidbody rigid;

    public bool isExplosion;

    private void Start()
    {
        isExplosion = false;
        StartCoroutine(Explosion());
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);

        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        meshObj.SetActive(false);
        isExplosion = true;

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0f, LayerMask.GetMask("User"));

        foreach (RaycastHit hitObj in rayHits)
        {
            hitObj.transform.GetComponent<UnitCs>().HitByBomb(transform.position);
        }

        Destroy(gameObject, 4);
    }
}
