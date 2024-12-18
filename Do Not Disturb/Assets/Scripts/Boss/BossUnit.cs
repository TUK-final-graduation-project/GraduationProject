using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BossUnit : MonoBehaviour
{
    public enum UnitState { Chase, Attack, Dead, Damaged };
    public enum BossType {  Rock, Wizard, Oak };


    [Header("Unit State")]
    public BossType type;
    public float HP = 1000f;
    float MaxHP = 1000f;
    public UnitState State;
    public GameObject target;
    public GameObject BossPoint;
    public float speed = 10;
    public bool isReady = false;

    public GameObject meshObj;
    public GameObject effectObj;

    Animator anim;
    Rigidbody rigid;

    [Header("Boss Attack")]
    public GameObject bullet;
    public GameObject indicator;

    Vector3[] path;
    int targetIndex;

    OurUnitController[] units;
    public GameObject BossAttack;
    public UnitStopZone stopzone;
    public GameManager manager;
    private CancellationTokenSource attackCts;

    private void Awake()
    {
        HP = MaxHP;
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        Invoke("RequestPathToMgr", 1);
    }

    // ============== 이동 ============== 

    public void RequestPathToMgr()
    {
        UnityEngine.Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
        transform.rotation = targetRotation;
        AstarManager.RequestPath(transform.position, target.transform.position, OnPathFound);
        State = UnitState.Chase;
    }
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful && this != null)
        {
            path = newPath;
            targetIndex = 0;
            FollowPath().Forget();
        }
    }
    async UniTaskVoid FollowPath()
    {
        Vector3 currentWaypoint;

        if (path.Length > 0)
        {
            currentWaypoint = path[0];
            currentWaypoint.y = 0; // y를 0으로 고정
            while (true)
            {
                if (this == null)
                {
                    return;
                }
                Vector3 direction = (currentWaypoint - transform.position).normalized;
                direction.y = 0; // y 방향은 0으로 고정
                if (Vector3.Distance(transform.position, currentWaypoint) < 0.5f)
                {
                    targetIndex++;
                    if (targetIndex >= path.Length)
                    {
                        isReady = true;
                        anim.SetTrigger("atBossPoint");
                        StartAttacking();

                        units = FindObjectsOfType(typeof(OurUnitController)) as OurUnitController[];
                        foreach (OurUnitController unit in units)
                        {
                            unit.target = this.gameObject;
                            unit.BossTargeting();
                        }
                        stopzone.StartMakeRock();
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

    // ============== 공격 ============== 

    void StartAttacking()
    {
        State = UnitState.Attack;
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
        while ( State == UnitState.Attack && !token.IsCancellationRequested)
        {
            anim.SetTrigger("DoAttack");
            await UniTask.Delay(5000, cancellationToken: token);
        }
    }

    // ============ 업데이트 ============ 

    void FreezeVelocity()
    {
        if (rigid.isKinematic == false || State != UnitState.Dead)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }
    private void FixedUpdate()
    {
        FreezeVelocity();

        switch (type)
        {
            case BossType.Rock:
                {
                    if (indicator.activeSelf == true)
                    {
                        if ( indicator.transform.localScale.x <= 40f)
                        {
                            indicator.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
                        }
                    }
                    break;
                }
            case BossType.Wizard:
                {
                    if ( indicator.activeSelf == true)
                    {
                        if (BossAttack != null && BossAttack.GetComponent<WizardBossAttack>().isExplosion == true)
                        {
                            indicator.SetActive(false);
                        }
                    }
                    break;
                }
            case BossType.Oak:
                {
                    if (indicator.activeSelf == true)
                    {
                        if (indicator.transform.localScale.x >= 30f)
                        {
                            indicator.transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f);
                        }
                    }
                    break;
                }
        }
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    // ============= 데미지 ============= 

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
            StartCoroutine(OnDamage(reactVec));
        }
    }

    IEnumerator OnDamage(Vector3 reactVec)
    {
        manager.UpdateBossHP(HP, MaxHP);
        effectObj.SetActive(true);

        if (HP <= 0)
        {
            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rigid.AddForce(reactVec * 5, ForceMode.Impulse);

            OnDestroy();
        }
        else
        {
            State = UnitState.Attack;

            yield return new WaitForSeconds(1f);

            effectObj.SetActive(false);
        }
    }
    public void PlayerDamage(Vector3 pos)
    {
        HP -= 100;
        Vector3 reactVec = transform.position - pos;
        StartCoroutine(OnDamage(reactVec));
    }
    public void OnDestroy()
    {
        State = UnitState.Dead;
        StopAttacking();
        anim.SetTrigger("DoDie");

        Destroy(gameObject, 3);
    }

    // =========== 애니메이션 =========== 

    private void MakeIndicator()
    {
        switch (type)
        {
            case BossType.Wizard:
                {
                    Vector3 pos = transform.position + new Vector3(Random.Range(-5f, 5f) * 10f, 0.5f, Random.Range(-5f, 5f) * 10f);

                    UnityEngine.Quaternion targetRotation = Quaternion.LookRotation(pos - transform.position);
                    transform.rotation = targetRotation;

                    indicator.SetActive(true);
                    indicator.transform.position = pos + Vector3.up;
                    break;
                }
            case BossType.Oak:
                {
                    Vector3 pos = transform.position + new Vector3(Random.Range(-5f, 5f) * 10f, 0.5f, Random.Range(-5f, 5f) * 10f);

                    UnityEngine.Quaternion targetRotation = Quaternion.LookRotation(pos - transform.position);
                    transform.rotation = targetRotation;

                    indicator.transform.localScale = new Vector3(40f, 40f, 40f);
                    indicator.SetActive(true);
                    indicator.transform.position = pos + Vector3.up;
                    break;
                }
            case BossType.Rock:
                {
                    //AudioManager.instance.PlaySfx(AudioManager.Sfx.Throw);
                    Vector3 pos = transform.position + new Vector3(Random.Range(-5f, 5f) * 10f, 0.5f, Random.Range(-5f, 5f) * 10f);

                    UnityEngine.Quaternion targetRotation = Quaternion.LookRotation(pos - transform.position);
                    transform.rotation = targetRotation;

                    indicator.transform.localScale = new Vector3(10f, 10f, 10f);
                    indicator.SetActive(true);
                    indicator.transform.position = pos + Vector3.up;
                    break;
                }
        }

    }
    private void MakeAttack()
    {
        switch (type)
        {
            case BossType.Wizard:
                {
                    //AudioManager.instance.PlaySfx(AudioManager.Sfx.Magic);
                    BossAttack = Instantiate(bullet, indicator.transform.GetChild(0).transform.position, Quaternion.identity);
                    BossAttack.GetComponent<WizardBossAttack>().FinalPosition = indicator.transform.GetChild(1).transform.position;
                    break;
                }
            case BossType.Oak:
                {
                    //AudioManager.instance.PlaySfx(AudioManager.Sfx.Wind);
                    BossAttack = Instantiate(bullet, indicator.transform.position, Quaternion.identity);
                    break;
                }
            case BossType.Rock:
                {
                    //AudioManager.instance.PlaySfx(AudioManager.Sfx.Bomb);
                    BossAttack = Instantiate(bullet, transform.position + Vector3.up * 15f, Quaternion.identity);
                    BossAttack.GetComponent<RockBossAttack>().TargetPosition = indicator.transform.position - Vector3.up;
                    break;
                }
        }

    }
}
