using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Unit : MonoBehaviour
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
    public GameObject attackTarget;
    public GameObject curTarget;
    bool isDamage;


    [Header("Melee Unit")]
    public BoxCollider meleeArea;

    [Header("Range Unit")]
    public GameObject bullet;

    Rigidbody rigid;
    BoxCollider boxCollider;

    NavMeshAgent nav;
    Animator anim;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        curTarget = target;

        Invoke("ChaseStart", 1);
    }
    private void Update()
    {
        if (nav.enabled)
        {
            if(curTarget == null)
            {
                attackTarget = null;
                curTarget = target;
            }
            nav.SetDestination(curTarget.transform.position);
            nav.isStopped = !isChase;
        }
    }
    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    void Targeting()
    {
        float targetRadius = 0;
        float targetRange = 0;

        if (type == Type.Melee)
        {
            targetRadius = 5f;
            targetRange = 12f;
        }
        else if (type == Type.Range)
        {
            targetRadius = 5f;
            targetRange = 25f;
        }
        RaycastHit[] rayHits = { };
        if ( team == Team.User)
        {
            rayHits =
                Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Com"));
        }
        else if(team == Team.Com)
        {
            rayHits =
                Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("User"));
        }
        if (rayHits.Length > 0 && !isAttack)
        {
            attackTarget = rayHits[0].collider.gameObject;
            // Debug.Log(attackTarget.name + " attack");
            curTarget = attackTarget;
            StartCoroutine(Attack());
        }
    }
    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

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
            GameObject instantBullet = Instantiate(bullet, transform.position, transform.rotation);
            Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
            rigidBullet.velocity = transform.forward * 20;

            yield return new WaitForSeconds(0.5f);
        }

        //isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
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
        if( attackTarget == null)
            Targeting();
        FreezeVelocity();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "UserAttack" && team == Team.Com)
        {
            if (!isDamage) // ���� ���°� �ƴ� ��
            {
                UnitAttack attack = other.GetComponent<UnitAttack>();
                HP -= attack.damage;
                if (other.GetComponent<Rigidbody>() != null) // ���Ÿ� ������ ���
                {
                    Destroy(other.gameObject);
                }
                StartCoroutine(OnDamage());
            }
        }
        else if(other.tag == "ComAttack" && team == Team.User)
        {
            if (!isDamage) // ���� ���°� �ƴ� ��
            {
                UnitAttack attack = other.GetComponent<UnitAttack>();
                HP -= attack.damage;
                if (other.GetComponent<Rigidbody>() != null) // ���Ÿ� ������ ���
                {
                    Destroy(other.gameObject);
                }
                StartCoroutine(OnDamage());
            }
        }
    }
    IEnumerator OnDamage()
    {
        isDamage = true;

        yield return new WaitForSeconds(0.1f);

        if(HP <= 0)
        {
            gameObject.layer = 12;
            anim.SetTrigger("doDie");
            isChase = false;
            nav.enabled = false;
            isDamage = false;

            Destroy(gameObject, 4);
        }
        else
        {
            yield return new WaitForSeconds(1f); // 1�� ����
            isDamage = false;
        }
    }
}
