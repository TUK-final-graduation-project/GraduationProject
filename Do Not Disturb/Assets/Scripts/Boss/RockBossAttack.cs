using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RockBossAttack : MonoBehaviour
{
    public GameObject meshObj;
    public Rigidbody rigid;

    public bool isExplosion;
    bool isArrive;

    public Vector3 TargetPosition;
    public GameObject effect;

    Vector3 dir;

    private void Start()
    {
        isExplosion = false;
        isArrive = false;
        dir = TargetPosition - transform.position;
        dir = dir.normalized;
    }

    private void Update()
    {
        if (!isArrive)
        {
            if ( transform.position.y >= 0)
            {
                transform.position += dir;
            }
            else
            {
                isArrive = true;

                StartCoroutine("Explosion");
            }
        }
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);

        isExplosion = true;
        meshObj.SetActive(false);
        effect.SetActive(true);
        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0f, LayerMask.GetMask("User"));

        foreach (RaycastHit hitObj in rayHits)
        {
            if (hitObj.transform.GetComponent<OurUnitController>() != null)
            {
                hitObj.transform.GetComponent<OurUnitController>().OnHitEnter("Bomb", transform.position);
            }
        }

        Destroy(gameObject, 4);
    }
}
