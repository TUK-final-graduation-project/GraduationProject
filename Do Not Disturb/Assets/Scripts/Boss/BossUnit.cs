using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossUnit : MonoBehaviour
{
    public enum UnitState { Chase, Attack, Targeting, Dead, Damaged };
    public enum BossType {  Rock, Wizard, Oak };


    [Header("Unit State")]
    public BossType type;
    public int HP = 1000;
    public UnitState State;
    public GameObject target;
    public GameObject BossPoint;
    public float speed = 10;

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

    public GameManager gameManager;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        units = FindObjectsOfType(typeof(OurUnitController)) as OurUnitController[];
        foreach(OurUnitController unit in units)
        {
            unit.target = BossPoint;
            unit.BossTargeting();
        }
        Invoke("RequestPathToMgr", 1);
    }

    public void RequestPathToMgr()
    {
        UnityEngine.Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
        transform.rotation = targetRotation;
        AstarManager.RequestPath(transform.position, target.transform.position, OnPathFound);
        State = UnitState.Chase;
    }
    // �� ã�� �����ϱ�
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
    // �����̱�
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
        yield return new WaitForSeconds(10f);
        
        State = UnitState.Attack;

        while (true)
        {
            anim.SetTrigger("isAttack");
            rigid.isKinematic = true;

            yield return new WaitForSeconds(6f);
            rigid.isKinematic = false;
        }
    }


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
                    BossAttack = Instantiate(bullet, indicator.transform.position, Quaternion.identity);
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Wind);
                    //BossAttack.GetComponent<OakBossAttack>().FinalPosition = indicator.transform.GetChild(1).transform.position;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "UserAttack")
        {
            if (State != UnitState.Damaged) // ���� ���°� �ƴ� ��
            {
                UnitAttack attack = other.GetComponent<UnitAttack>();
                HP -= attack.damage;
                if (other.GetComponent<Rigidbody>() != null) // ���Ÿ� ������ ���
                {
                    Destroy(other.gameObject);
                }
                Vector3 reactVec = transform.position - other.transform.position;
                StartCoroutine(OnDamage(reactVec, false));
            }
        }
        if ( other.tag == "User")
        {
            other.gameObject.GetComponent<OurUnitController>().StopTpBossPoint();
            // ���⼭ �����̴� �� ���߱�
        }
    }

    IEnumerator OnDamage(Vector3 reactVec, bool isGrenade)
    {
        yield return new WaitForSeconds(0.1f);

        UnitState tmp = State;
        State = UnitState.Damaged;

        //gameManager.UpdateBossHP();

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
            yield return new WaitForSeconds(1f); // 1�� ����
            State = tmp;
        }
    }

    public void OnDestroy()
    {
        anim.SetTrigger("isDie");
        State = UnitState.Dead;
        // effectObj.SetActive(true);

        Destroy(gameObject, 3);
    }
}
