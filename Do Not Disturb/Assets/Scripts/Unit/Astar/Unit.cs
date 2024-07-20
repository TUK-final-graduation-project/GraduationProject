using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Unit : MonoBehaviour
{
    public enum Type { Melee, Range };
    public enum Team { User, Com };
    public enum UnitState { Chase, Attack, Targeting, Dead, Damaged };

    [Header("Unit Type")]
    public Type type;
    public Team team;

    [Header("Unit State")]
    public int HP;
    public UnitState State;
    public GameObject target;
    public GameObject Base;
    public float speed = 10;
    Animator anim;
    Rigidbody rigid;
    Tower[] towers;

    [Header("Melee Unit")]
    public BoxCollider meleeArea;
    public bool isDear;

    [Header("Range Unit")]
    public GameObject bullet;

    Vector3[] path;
    int targetIndex;

    public GameObject meshObj;
    public GameObject effectObj;


    public GameObject[] bossList;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        StartCoroutine(MoveUnit());
    }

    IEnumerator MoveUnit()
    {
        CalculateUnitTarget();

        yield return new WaitForSeconds(5f);
    }

    private void CalculateUnitTarget()
    {
        // 3순위: 각 베이스
        target = Base;

        // 2순위: 유닛


        // 1순위: 각 보스
        foreach ( GameObject boss in bossList)
        {
            if ( boss.activeSelf)
            {
                target = boss;
                break;
            }
        }

        
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
        float targetRadius = 0;
        float targetRange = 0;

        if (type == Type.Melee)
        {
            targetRadius = 1f;
            targetRange = 3f;
        }
        else if (type == Type.Range)
        {
            targetRadius = 0.5f;
            targetRange = 25f;
        }
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
            if (type == Type.Melee)
            {
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(0.5f);
                rigid.velocity = Vector3.zero;
                meleeArea.enabled = false;

                yield return new WaitForSeconds(0.5f);
            }
            else if (type == Type.Range)
            {
                yield return new WaitForSeconds(0.5f);

                GameObject instantBullet = Instantiate(bullet, transform.position + Vector3.up * 1.5f, Quaternion.identity);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();

                Vector3 direction = (target.transform.position - transform.position).normalized;
                rigidBullet.AddForce(direction * 10, ForceMode.Impulse);

                yield return new WaitForSeconds(2f);
            }

            anim.SetBool("isAttack", false);
            rigid.isKinematic = false;
            State = UnitState.Targeting;

            if (target == null || target == Base)
            {
                State = UnitState.Chase;
                //if ( isDear )
                //{
                //    HP = -10;
                //    Vector3 reactVec = transform.position - target.transform.position;
                //    StartCoroutine(OnDamage(reactVec, false));
                //}
                yield break;
            }

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

    void FindNextTarget()
    {
        target = Base;
        float minDistance = Vector3.Distance(transform.position, Base.transform.position);
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
                StopCoroutine("Attack");
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

    public void HitByWizardBoss()
    {
        HP -= 10;
        if (HP <= 0)
        {
            gameObject.layer = gameObject.layer + 1;
            OnDestroy();
        }
    }

    public void HitByOakBoss()
    {
        StopCoroutine("FollowPath");
        StopCoroutine("Attack");

        StartCoroutine("DamagedByBoss");
    }
    IEnumerator DamagedByBoss()
    {
        while (true)
        {
            transform.position += Vector3.up;

            if (transform.position.y >= 30f)
            {
                break;
            }
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        while (true)
        {
            transform.position -= Vector3.up * 3;

            if (transform.position.y <= 0f)
            {
                OnDestroy();
                yield break;
            }
            yield return null;
        }
    }
}
