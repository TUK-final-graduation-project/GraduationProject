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
        // collider ���� Ȥ�� unit tag�� �����ؼ�
        // ������ ���� ���� ������ ����ġ��
        // stopcoroutine ȣ���ϸ�
        // �� ã�� ����
        // �ο�� �����ϰ�
        // ���� �ο�� ��밡 �׾��� ���
        // ���Ӱ� �� ã�� ���� >> RequestPath(���� ��ġ, target, OnPathFound)
        StopCoroutine("FollowPath");
    }
    // ã�ƾ� �� �� ��û�ϱ�
    private void Start()
    {
        // target = GameObject.Find("FireBase");
        AstarManager.RequestPath(transform.position, target, OnPathFound);

    }

    // �� ã�� �����ϱ�
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    // �����̱�
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
