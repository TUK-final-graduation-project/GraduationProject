using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossUnit : MonoBehaviour
{
    public enum UnitState { Chase, Attack, Targeting, Dead, Damaged };

    [Header("Unit State")]
    public int HP;
    public UnitState State;
    public GameObject target;
    public GameObject BossPoint;
    public float speed = 10;

    Animator anim;
    Rigidbody rigid;

    public GameObject bullet;
    public GameObject indicator;

    Vector3[] path;
    int targetIndex;

    public GameObject meshObj;
    public GameObject effectObj;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        Invoke("RequestPathToMgr", 1);
    }

    public void RequestPathToMgr()
    {
        UnityEngine.Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
        transform.rotation = targetRotation;
        AstarManager.RequestPath(transform.position, target.transform.position, OnPathFound);
        State = UnitState.Chase;
    }
    // 길 찾기 시작하기
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful && this != null)
        {
            path = newPath;
            targetIndex = 0;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }
    // 움직이기
    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint;

        if (path.Length > 0)
        {
            currentWaypoint = path[0];

            while (true)
            {
                if ((int)transform.position.x == (int)currentWaypoint.x && (int)transform.position.z == (int)currentWaypoint.z)
                {
                    targetIndex++;
                    if (targetIndex >= path.Length)
                    {
                        anim.SetTrigger("atBossPoint");
                        StartCoroutine("Attack");
                        yield break;
                    }
                    currentWaypoint = path[targetIndex];
                }
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                yield return null;
            }
        }
    }

    IEnumerator Attack()
    {
        State = UnitState.Attack;
        anim.SetBool("isAttack1", true);
        rigid.isKinematic = true;

        while (true)
        {
            yield return new WaitForSeconds(0.5f);

            Vector3 pos = transform.position + new Vector3(Random.Range(-30f, 30f), 0, Random.Range(-30f, 30f));
            GameObject instantBullet = Instantiate(bullet, pos + Vector3.up * 30f, Quaternion.identity);
            Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
            indicator.SetActive(true);
            indicator.transform.position = pos;

            //Vector3 direction = (target.transform.position - transform.position).normalized;
            //rigidBullet.AddForce(direction * 10, ForceMode.Impulse);

            yield return new WaitForSeconds(2f);

            anim.SetBool("isAttack1", false);
            rigid.isKinematic = false;

            yield return new WaitForSeconds(0.5f);

            anim.SetBool("isAttack2", true);
            rigid.isKinematic = true;

            yield return new WaitForSeconds(2f);

            anim.SetBool("isAttack2", false);
            rigid.isKinematic = false;

            yield return new WaitForSeconds(0.5f);

            anim.SetBool("isAttack1", true);
            rigid.isKinematic = true;
        }
    }

    void FreezeVelocity()
    {
        if (rigid.isKinematic == false)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    private void FixedUpdate()
    {
        if (State != UnitState.Dead)
        {
            FreezeVelocity();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ComAttack")
        {
            if (State != UnitState.Damaged) // 무적 상태가 아닐 때
            {
                UnitAttack attack = other.GetComponent<UnitAttack>();
                HP -= attack.damage;
                if (other.GetComponent<Rigidbody>() != null) // 원거리 공격인 경우
                {
                    Destroy(other.gameObject);
                }
                Vector3 reactVec = transform.position - other.transform.position;
                StartCoroutine(OnDamage(reactVec, false));
            }
        }
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        yield return new WaitForSeconds(0.1f);

        UnitState tmp = State;
        State = UnitState.Damaged;

        if (HP <= 0)
        {
            gameObject.layer = gameObject.layer + 1;

            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * 5, ForceMode.Impulse);

            OnDestroy();
        }
        else
        {
            yield return new WaitForSeconds(1f); // 1초 무적
            State = tmp;
        }
    }

    public void OnDestroy()
    {
        anim.SetBool("isDie", true);
        State = UnitState.Dead;
        // effectObj.SetActive(true);

        Destroy(gameObject, 3);
    }
}
