using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMove : MonoBehaviour
{
    //public Transform target;
    public Vector3 target;
    float speed = 5;
    Vector3[] path;
    int targetIndex;

    // 찾아야 할 길 요청하기
    private void Start()
    {
        // target = GameObject.Find("FireBase");
        RequestPathToMgr();

    }
    public void RequestPathToMgr()
    {
        AstarManager.RequestPath(transform.position, target, OnPathFound);
        Debug.Log(transform.position);
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
}
