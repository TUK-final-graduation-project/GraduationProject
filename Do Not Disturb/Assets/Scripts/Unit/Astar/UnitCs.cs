using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

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
    public GameObject UserBase;
    public Tower[] towers;

    [Header("Melee Unit")]
    public BoxCollider meleeArea;

    [Header("Range Unit")]
    public GameObject bullet;
    public Transform BulletSpawnPoint;

    Rigidbody rigid;

    public Animator anim;

    public float speed = 10;
    Vector3[] path;
    int targetIndex;

    public GameObject meshObj;
    public GameObject effectObj;

    bool isDead = false;
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
        isChase = true;
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
        Vector3 currentWaypoint = path[0];

        while (true)
        {
            //Debug.Log("cur: " + currentWaypoint + ", unit: " + transform.position);
            //Debug.Log("int z: " + (int)transform.position.z);
            //Debug.Log("int cur z: " + (int)currentWaypoint.z);
            if ((int)transform.position.x == (int)currentWaypoint.x && (int)transform.position.z == (int)currentWaypoint.z)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    Debug.Log("?");
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
            StopCoroutine("FollowPath");
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;

        if (type == Type.Melee)
        {
            //yield return new WaitForSeconds(0.1f);
            //rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
            //meleeArea.enabled = true;

            //yield return new WaitForSeconds(0.5f);
            //rigid.velocity = Vector3.zero;
            //meleeArea.enabled = false;

            //yield return new WaitForSeconds(0.5f);
        }
        else if (type == Type.Range)
        {
            yield return new WaitForSeconds(0.5f);
            //Debug.Log(transform.forward);
            //Debug.Log(transform.rotation);
            GameObject instantBullet = Instantiate(bullet, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();

            Vector3 direction = (target.transform.position - transform.position).normalized;
            rigidBullet.AddForce(direction * 10, ForceMode.Impulse);


            yield return new WaitForSeconds(2f);
        }

        isChase = true;
        isAttack = false;
        rb.isKinematic = false;
        anim.SetBool("isAttack", false);
        FindNextTarget();
    }

    void FreezeVelocity()
    {
        if (isChase)
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
        if (!isDead)
        {
            if ( target == null )
            {
                FindNextTarget();
            }
            Targeting();
            FreezeVelocity();

        }
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
            
            isChase = false;
            // nav.enabled = false;
            isDamage = false;

            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * 5, ForceMode.Impulse);

            OnDestroy();
        }
        else
        {
            yield return new WaitForSeconds(1f); // 1초 무적
            isDamage = false;
            isChase = true;
            StartCoroutine("FollowPath");
        }
    }
    public void OnDestroy()
    {
        anim.SetBool("isDie", true);
        StopCoroutine("FollowPath");
        isDead = true;
       //  meshObj.SetActive(false);
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
