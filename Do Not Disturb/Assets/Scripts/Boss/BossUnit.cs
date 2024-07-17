using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossUnit : MonoBehaviour
{
    public enum UnitState { Chase, Attack, Targeting, Dead, Damaged };

    [Header("Unit State")]
    public int HP;
    public UnitState State;
    public GameObject target;
    public GameObject BossPoint;
    public float speed = 10;

    Animator anim;
    Rigidbody rigid;

    public BoxCollider meleeArea;

    Vector3[] path;
    int targetIndex;

    public GameObject meshObj;
    public GameObject effectObj;

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
    }
}
