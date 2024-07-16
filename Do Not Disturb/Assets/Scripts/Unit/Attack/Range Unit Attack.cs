using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class RangeUnitAttack : MonoBehaviour
{
    public enum Team { User, Com };
    public enum UnitState { Chase, Attack, Targeting, Dead, Damaged };

    [Header("Unit Type")]
    public Team team;

    [Header("Unit State")]
    public int HP;
    public UnitState State;
    public GameObject target;
    public GameObject UserBase;
    public float speed = 10;
    Animator anim;
    Rigidbody rigid;
    Tower[] towers;

    [Header("Range Unit")]
    public GameObject bullet;

    Vector3[] path;
    int targetIndex;

    public GameObject meshObj;
    public GameObject effectObj;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        State = UnitState.Chase;
        Invoke("RequestPathToMgr", 1);
    }

    public void RequestPathToMgr()
    {
        UnityEngine.Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
        transform.rotation = targetRotation;
        AstarManager.RequestPath(transform.position, target.transform.position, OnPathFound);
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
                        yield break;
                    }
                    currentWaypoint = path[targetIndex];
                }
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                yield return null;
            }
        }
    }


    void Targeting()
    {
        float targetRadius = 0.5f;
        float targetRange = 25f;

        RaycastHit[] rayHits = { };
        if (team == Team.User)
        {
            rayHits =
                Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Com"));
        }
        else if (team == Team.Com)
        {
            rayHits =
                Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("User"));
        }
        if (rayHits.Length > 0 && State != UnitState.Attack)
        {
            StopCoroutine("FollowPath");
            target = rayHits[0].collider.gameObject;
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        State = UnitState.Attack;
        anim.SetBool("isAttack", true);
        rigid.isKinematic = true;

        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            Debug.Log("ss");
            yield break;

        }

        //GameObject instantBullet = Instantiate(bullet, transform.position + Vector3.up * 1.5f, Quaternion.identity);
        //Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();

        //Vector3 direction = (target.transform.position - transform.position).normalized;
        //rigidBullet.AddForce(direction * 10, ForceMode.Impulse);

        //yield return new WaitForSeconds(2f);

        //anim.SetBool("isAttack", false);
        //rigid.isKinematic = false;
        //if (target != null)
        //{
        //    State = UnitState.Targeting;
        //    RequestPathToMgr();
        //}
    }

    void FreezeVelocity()
    {
        if (rigid.isKinematic == false)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void FindNextTarget()
    {
        target = UserBase;
        float minDistance = Vector3.Distance(transform.position, UserBase.transform.position);
        float tmp;
        towers = FindObjectsOfType(typeof(Tower)) as Tower[];
        foreach (Tower tower in towers)
        {
            tmp = Vector3.Distance(transform.position, tower.transform.position);
            if (minDistance > tmp)
            {
                minDistance = tmp;
                target = tower.gameObject;
            }
        }
        RequestPathToMgr();
    }

    private void FixedUpdate()
    {
        if (State != UnitState.Dead)
        {
            if (target == null)
            {
                FindNextTarget();
            }
            if (State != UnitState.Targeting)
            {
                Targeting();
            }
            FreezeVelocity();

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "UserAttack" && team == Team.Com)
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
        else if (other.tag == "ComAttack" && team == Team.User)
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
        StopCoroutine("FollowPath");
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
            StartCoroutine("FollowPath");
        }
    }

    public void OnDestroy()
    {
        anim.SetBool("isDie", true);
        StopCoroutine("FollowPath");
        State = UnitState.Dead;
        // effectObj.SetActive(true);

        Destroy(gameObject, 3);
    }

    public void HitByBomb(Vector3 explosionPos)
    {
        HP -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));
    }
}
