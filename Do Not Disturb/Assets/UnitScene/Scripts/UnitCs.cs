using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitCs : MonoBehaviour
{
    public enum Type { Melee, Range };
    public enum Team { User, Com };

    [Header("Unit Type")]
    public Type type;
    public Team team;

    [Header("Unit State")]
    public int HP;
    public bool isChase;
    public bool isAttack;
    public GameObject target;
    bool isDamage;


    [Header("Melee Unit")]
    public BoxCollider meleeArea;

    [Header("Range Unit")]
    public GameObject bullet;

    Rigidbody rigid;
    BoxCollider boxCollider;

    public Animator anim;

    float speed = 5;
    Vector3[] path;
    int targetIndex;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        // anim = GetComponentInChildren<Animator>();

        Invoke("RequestPathToMgr", 1);
    }

    public void RequestPathToMgr()
    {
        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
        transform.rotation = targetRotation;
        AstarManager.RequestPath(transform.position, target.transform.position, OnPathFound);
       // anim.SetBool("isWalk", true);
        isChase = true;
    }

    // 길 찾기 시작하기
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    // 움직이기
    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];

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


    void Targeting()
    {
        float targetRadius = 0;
        float targetRange = 0;

        if (type == Type.Melee)
        {
            targetRadius = 1f;
            targetRange = 12f;
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
        if (rayHits.Length > 0 && !isAttack)
        {
            // Debug.Log(rayHits[0].collider.gameObject.name);
            StartCoroutine(Attack());
            StopCoroutine("FollowPath");
        }
    }
    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
       // anim.SetBool("isAttack", true);

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
            //Debug.Log(transform.forward);
            //Debug.Log(transform.rotation);
            GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
            Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
            rigidBullet.velocity = transform.forward * 50;

            yield return new WaitForSeconds(2f);
        }

        isChase = true;
        isAttack = false;
       // anim.SetBool("isAttack", false);
        StartCoroutine("FollowPath");
    }
    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }
    private void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "UserAttack" && team == Team.Com)
        {
            if (!isDamage) // 무적 상태가 아닐 때
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
            if (!isDamage) // 무적 상태가 아닐 때
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
        isDamage = true;
        isChase = false;
        StopCoroutine("FollowPath");
        yield return new WaitForSeconds(0.1f);

        if (HP <= 0)
        {
            gameObject.layer = gameObject.layer + 1;
          //  anim.SetTrigger("doDie");
            isChase = false;
            // nav.enabled = false;
            isDamage = false;

            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * 5, ForceMode.Impulse);

            Destroy(gameObject);
        }
        else
        {
            yield return new WaitForSeconds(1f); // 1초 무적
            isDamage = false;
            isChase = true;
            StartCoroutine("FollowPath");
        }
    }

    public void HitByBomb(Vector3 explosionPos)
    {
        HP -= 100;
        Vector3 reactVec = transform.position - explosionPos;
        StartCoroutine(OnDamage(reactVec, true));
    }
}
