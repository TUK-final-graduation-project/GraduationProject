using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static UnitCs;

public class OurUnitController : MonoBehaviour
{
    public enum Type { Melee, Range };
    public enum UnitState { Walk, Targeting, Attack, GoToBoss };

    [Header("Unit")]
    Animator anim;
    Rigidbody rigid;
    public UnitState state;
    public Type type;
    public int HP;

    [Header("Unit Chase")]
    public int speed = 5;
    public GameObject target;
    public GameObject Base;
    Vector3[] path;
    int targetIndex;

    [Header("Melee Unit")]
    public GameObject meleeArea;

    [Header("Range Unit")]
    public GameObject bullet;

    private CancellationTokenSource pathFindingCts;
    private CancellationTokenSource targetingCts;
    private CancellationTokenSource attackCts;

    private void Awake()
    {
        target = Base;
        state = UnitState.Walk;
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();

        BossUnit[] bosses = FindObjectsOfType(typeof(BossUnit)) as BossUnit[];
        foreach(BossUnit boss in bosses)
        {
            if ( boss.isReady == true)
            {
                target = boss.gameObject;
                break;
            }
        }
        Invoke("RequestPathToMgr", 1);
        StartTargeting();
    }

    ///////////////// 길 찾기 ///////////////////// 
    void StartPathFinding()
    {
        pathFindingCts = new CancellationTokenSource();
        FollowPath(pathFindingCts.Token).Forget();
    }
    void StopPathFinding()
    {
        if (pathFindingCts != null)
        {
            pathFindingCts.Cancel();
            pathFindingCts.Dispose();
            pathFindingCts = null;
        }
    }
    public void RequestPathToMgr()
    {
        UnityEngine.Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
        transform.rotation = targetRotation;
        AstarManager.RequestPath(transform.position, target.transform.position, OnPathFound);
    }
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful && this != null)
        {
            path = newPath;
            targetIndex = 0;
            StartPathFinding();
        }
    }
    async UniTaskVoid FollowPath(CancellationToken token)
    {
        Vector3 currentWaypoint;

        if (path.Length > 0)
        {
            currentWaypoint = path[0];
            currentWaypoint.y = 0; // y를 0으로 고정
            while (!token.IsCancellationRequested)
            {
                if (this == null)
                {
                    return;
                }
                Vector3 direction = (currentWaypoint - transform.position).normalized;
                direction.y = 0; // y 방향은 0으로 고정
                if (Vector3.Distance(transform.position, currentWaypoint) < 0.1f)
                {
                    targetIndex++;
                    if (targetIndex >= path.Length)
                    {
                        if ( state == UnitState.GoToBoss )
                        {
                            StartAttacking();
                        }
                        return;
                    }
                    currentWaypoint = path[targetIndex];
                    currentWaypoint.y = 0; // y를 0으로 고정
                    transform.rotation = Quaternion.LookRotation(currentWaypoint - transform.position);
                }
                else
                {
                    transform.position += direction * speed * Time.deltaTime;
                }
                //transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
                await UniTask.Yield();
            }
        }
    }
    //////////////////////////////////////////////

    ///////////////// 타겟팅 //////////////////////
    void StartTargeting()
    {
        targetingCts = new CancellationTokenSource();
        Targeting(targetingCts.Token).Forget();
    }
    void StopTargeting()
    {
        if (targetingCts != null)
        {
            targetingCts.Cancel();
            targetingCts.Dispose();
            targetingCts = null;
        }
    }
    async UniTaskVoid Targeting(CancellationToken token)
    {
        while (state == UnitState.Walk && !token.IsCancellationRequested)
        {
            if (this == null)
            {
                return;
            }
            FindUnit();

            await UniTask.Delay(1000, cancellationToken: token);
        }
    }
    private void FindUnit()
    {
        float targetRadius = 0;
        float targetRange = 0;

        if (type == Type.Melee)
        {
            targetRadius = 4f;
            targetRange = 3f;
        }
        else if (type == Type.Range)
        {
            targetRadius = 5f;
            targetRange = 25f;
        }
        RaycastHit[] rayHits =
                Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Com"));

        if (rayHits.Length > 0)
        {
            target = rayHits[0].collider.gameObject;
            StartAttacking();
        }
    }
    //////////////////////////////////////////////

    /////////////////// 공격 //////////////////////
    void StartAttacking()
    {
        StopPathFinding();
        state = UnitState.Attack;
        attackCts = new CancellationTokenSource();
        Attack(attackCts.Token).Forget();
    }
    void StopAttacking()
    {
        if (attackCts != null)
        {
            attackCts.Cancel();
            attackCts.Dispose();
            attackCts = null;
        }
    }
    async UniTask Attack(CancellationToken token)
    {
        while (state == UnitState.Attack && !token.IsCancellationRequested)
        {
            if ( this == null )
            {
                return;
            }
            anim.SetTrigger("DoAttack");
            await UniTask.Delay(1000, cancellationToken: token);

            if (target == null)
            {
                state = UnitState.Walk;
                target = Base;
                RequestPathToMgr();
                return;
            }
        }
    }
    public void StartAttack()
    {

        rigid.isKinematic = true;

        if (type == Type.Melee)
        {
            //rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
            meleeArea.SetActive(true);
        }
        else if (type == Type.Range)
        {

            GameObject instantBullet = Instantiate(bullet, transform.position + Vector3.up * 1.5f, transform.rotation);
            Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();

            Vector3 direction = (target.transform.position - transform.position).normalized;
            rigidBullet.velocity = Vector3.forward * 50f;

        }
    }
    public void EndAttack()
    {
        rigid.isKinematic = false;
        rigid.velocity = Vector3.zero;
        if (type == Type.Melee)
        {
            meleeArea.SetActive(false);
        }

    }
    //////////////////////////////////////////////

    //////////////////////////////////////////////
    void FreezeVelocity()
    {
        if ( rigid.isKinematic == false)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }
    private void FixedUpdate()
    {
        FreezeVelocity();
    }
    public void OnDestroy()
    {
        StopPathFinding();
        StopAttacking();
        StopTargeting();
        anim.SetTrigger("DoDie");

        Destroy(gameObject, 2);
    }
    //////////////////////////////////////////////

    /////////////////// 보스 ///////////////////////
    public void BossTargeting()
    {
        state = UnitState.GoToBoss;
        StopAttacking();
        StopPathFinding();
        RequestPathToMgr();
    }
    public void StopToBossPoint()
    {
        StopPathFinding();
        StartAttacking();
    }
    //////////////////////////////////////////////
    
    ////////////////// 데미지 /////////////////////
    private void OnDamage(Vector3 reactVec)
    {
        if (HP <= 0)
        {
            gameObject.layer = gameObject.layer + 1;

            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * 5, ForceMode.Impulse);

            OnDestroy();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ComAttack")
        {
            UnitAttack attack = other.GetComponent<UnitAttack>();
            HP -= attack.damage;
            if (other.GetComponent<Rigidbody>() != null) // 원거리 공격인 경우
            {
                Destroy(other.gameObject);
            }
            Vector3 reactVec = transform.position - other.transform.position;
            OnDamage(reactVec);
        }
    }
    public void OnHitEnter(string type, Vector3 explosionPos)
    {
        switch (type)
        {
            case "Oak":
                {
                    StopAttacking();
                    StopPathFinding();
                    StopTargeting();

                    DamagedByOak().Forget();
                    break;
                }
            case "Wizard":
                {
                    HP -= 10;
                    if (HP <= 0)
                    {
                        gameObject.layer = gameObject.layer + 1;
                        OnDestroy();
                    }
                    break;
                }
            case "Bomb":
                {
                    HP -= 100;
                    Vector3 reactVec = transform.position - explosionPos;
                    OnDamage(reactVec);
                    break;
                }
        }
    }
    async UniTaskVoid DamagedByOak()
    {
        while(true)
        {
            if (this == null)
            {
                return;
            }
            transform.position += Vector3.up;

            if (transform.position.y >= 30f)
            {
                break;
            }
            await UniTask.Yield();
        }

        await UniTask.Delay(1000);

        while (true)
        {
            transform.position -= Vector3.up * 3;

            if (transform.position.y <= 0f)
            {
                OnDestroy();
                return;
            }
            await UniTask.Yield();
        }
    }
    //////////////////////////////////////////////
    
    public void SetSpeed(int _speed)
    {
        speed = _speed;
    }

    public void SetHP(int _hp) 
    {
        HP = _hp;
    }

}
