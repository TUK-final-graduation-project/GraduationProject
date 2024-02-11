using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserUnit : MonoBehaviour
{
    //public Transform target;
    public Vector3 target;
    float speed = 5;
    Vector3[] path;
    int targetIndex;

    private void OnCollisionEnter(Collision collision)
    {
        // collider 조정 혹은 unit tag로 구분해서
        // 유닛이 일정 범위 내에서 마주치면
        // stopcoroutine 호출하면
        // 길 찾기 멈춤
        // 싸우는 행위하고
        // 만약 싸우던 상대가 죽었을 경우
        // 새롭게 길 찾기 시작 >> RequestPath(현재 위치, target, OnPathFound)
        StopCoroutine("FollowPath");
    }
    // 찾아야 할 길 요청하기
    private void Start()
    {
        // target = GameObject.Find("FireBase");
        AstarManager.RequestPath(transform.position, target, OnPathFound);

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
            if (transform.position == currentWaypoint)
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
