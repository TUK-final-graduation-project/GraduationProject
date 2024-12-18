using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86.Avx;

public class EnemyUnitController : MonoBehaviour
{
    public enum Type { Melee, Range };
    public enum UnitState { Walk, Targeting, Attack, GoToBoss, Damaged };

    [Header("Unit")]
    Animator anim;
    Rigidbody rigid;
    public UnitState state;
    public Type type;
    public int HP;
    public GameObject DieEffect;

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
        Invoke("StartFindTower", 1);
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
            currentWaypoint.y = 0; // y를 0으로 고정
            while (!token.IsCancellationRequested)
            {
                if (this == null)
                {
                    return;
                }

                if ( target == null)
                {
                    StopPathFinding();
                    target = Base;
                    RequestPathToMgr();
                    return;
                }
                Vector3 direction = (currentWaypoint - transform.position).normalized;
                direction.y = 0; // y 방향은 0으로 고정
                if (Vector3.Distance(transform.position, currentWaypoint) < 0.5f)
                {
                    targetIndex++;
                    if (targetIndex >= path.Length)
                    {
                        StopPathFinding();
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
                Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("User"));

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
    void StopFindTower()
    {
        if (FindingCts != null)
        {
            FindingCts.Cancel();
            FindingCts.Dispose();
            FindingCts = null;
        }
    }
    async UniTaskVoid Finding(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
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
        while (!token.IsCancellationRequested)
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
            // rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
            meleeArea.enabled = true;
        }
        else if (type == Type.Range)
        {
            if ( target != null )
            {
                GameObject instantBullet = Instantiate(bullet, transform.position + Vector3.up * 1.5f, Quaternion.identity);
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();

                Vector3 direction = (target.transform.position - transform.position).normalized;
                rigidBullet.AddForce(direction * 10, ForceMode.Impulse);
            }

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
        if ( rigid.isKinematic == false)
        { 
            FreezeVelocity(); 
        }
    }
    public void OnDestroy()
    {
        StopPathFinding();
        StopAttacking();
        StopTargeting();
        StopFindTower();
        anim.SetTrigger("DoDie");
        if ( DieEffect != null)
        {
            DieEffect.SetActive(true);
        }
        Destroy(gameObject, 2);
    }
    //////////////////////////////////////////////

    ////////////////// 데미지 /////////////////////
    private void OnDamage(Vector3 reactVec)
    {
        if (HP <= 0 && state != UnitState.Damaged)
        {
            gameObject.layer = gameObject.layer + 1;

            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * 5, ForceMode.Impulse);
            state = UnitState.Damaged;
            OnDestroy();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "UserAttack")
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
            case "Bomb":
                {
                    HP -= 100;
                    Vector3 reactVec = transform.position - explosionPos;
                    OnDamage(reactVec);
                    break;
                }
            case "Slow":
                {
                    if ( speed > 1)
                    {
                        speed -= 1;
                    }
                    break;
                }
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
