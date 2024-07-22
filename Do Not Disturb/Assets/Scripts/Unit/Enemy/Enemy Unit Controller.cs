using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public class EnemyUnitController : MonoBehaviour
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
    Tower[] towers;

    [Header("Melee Unit")]
    public BoxCollider meleeArea;

    [Header("Range Unit")]
    public GameObject bullet;

    private CancellationTokenSource pathFindingCts;
    private CancellationTokenSource targetingCts;
    private CancellationTokenSource attackCts;
    private CancellationTokenSource FindingCts;

    private void Awake()
    {
        target = Base;
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody>();
        Invoke("RequestPathToMgr", 1);
        StartTargeting();
        StartFindTower();
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
            state = UnitState.Walk;
            StartPathFinding();
        }
    }
    async UniTaskVoid FollowPath(CancellationToken token)
    {
        Vector3 currentWaypoint;

        if (path.Length > 0)
        {
            currentWaypoint = path[0];

            while (!token.IsCancellationRequested)
            {
                if ((int)transform.position.x == (int)currentWaypoint.x && (int)transform.position.z == (int)currentWaypoint.z)
                {
                    targetIndex++;
                    if (targetIndex >= path.Length)
                    {
                        return;
                    }
                    currentWaypoint = path[targetIndex];
                }
                transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
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
            targetRadius = 1f;
            targetRange = 3f;
        }
        else if (type == Type.Range)
        {
            targetRadius = 0.5f;
            targetRange = 25f;
        }
        RaycastHit[] rayHits =
                Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Com"));

        if (rayHits.Length > 0)
        {
            state = UnitState.Attack;
            target = rayHits[0].collider.gameObject;
            StartAttacking();
        }
    }
    //////////////////////////////////////////////

    ///////////////// 타워 찾기 //////////////////////
    void StartFindTower()
    {
        FindingCts = new CancellationTokenSource();
        Finding(FindingCts.Token).Forget();
    }
    async UniTaskVoid Finding(CancellationToken token)
    {
        while (state == UnitState.Walk && !token.IsCancellationRequested)
        {
            if ( target == Base || target == null)
            {
                FindTower();
            }
            else if ( target != Base)
            {
                StopPathFinding();
                RequestPathToMgr();
            }
            await UniTask.Delay(2000, cancellationToken: token);
        }
    }
    private void FindTower()
    {
        towers = FindObjectsOfType(typeof(Tower)) as Tower[];
        float minDistance = Vector3.Distance(transform.position, Base.transform.position);
        foreach (Tower tower in towers)
        {
            float tmp = Vector3.Distance(transform.position, tower.transform.position);
            if (minDistance > tmp)
            {
                minDistance = tmp;
                target = tower.gameObject;
            }
        }
    }
    //////////////////////////////////////////////

    /////////////////// 공격 //////////////////////
    void StartAttacking()
    {
        StopPathFinding();
        StopTargeting();
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
            anim.SetTrigger("DoAttack");

            await UniTask.Delay(1000, cancellationToken: token);

            if (target == null)
            {
                state = UnitState.Walk;
                target = Base;
                RequestPathToMgr();
                StartTargeting();
                return;
            }
        }
    }
    public void StartAttack()
    {

        rigid.isKinematic = true;

        if (type == Type.Melee)
        {
            rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
            meleeArea.enabled = true;
        }
        else if (type == Type.Range)
        {

            GameObject instantBullet = Instantiate(bullet, transform.position + Vector3.up * 1.5f, Quaternion.identity);
            Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();

            Vector3 direction = (target.transform.position - transform.position).normalized;
            rigidBullet.AddForce(direction * 10, ForceMode.Impulse);

        }
    }
    public void EndAttack()
    {
        if (type == Type.Melee)
        {
            rigid.velocity = Vector3.zero;
            meleeArea.enabled = false;
        }

        rigid.isKinematic = false;
    }
    //////////////////////////////////////////////

    //////////////////////////////////////////////
    void FreezeVelocity()
    {
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
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
        while (true)
        {
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
}
