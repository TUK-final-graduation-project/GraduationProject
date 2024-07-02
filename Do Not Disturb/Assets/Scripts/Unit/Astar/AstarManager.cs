using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class AstarManager : MonoBehaviour
{
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static AstarManager instance;
    PathFinding pathFinding;

    bool isProcessingPath;

    private void Awake()
    {
        instance = this;
        pathFinding = GetComponent<PathFinding>();
    }

    // 길 찾기 큐에 찾아야 할 길 넣기
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, UnityAction<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.pathRequestQueue.Enqueue(newRequest);
        instance.TryProcessNext();
    }

    // 큐에서 다음 찾아야 할 길 꺼내오기
    void TryProcessNext()
    {
        if(!isProcessingPath && pathRequestQueue.Count > 0)
        {
            currentPathRequest = pathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathFinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }

    // 길 찾기 종료 후 TryProcessNext 호출하기
    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        // 움직이기
        currentPathRequest.callback(path, success);
        isProcessingPath = false;
        TryProcessNext();   
    }

}

struct PathRequest
{
    public Vector3 pathStart;
    public Vector3 pathEnd;
    public UnityAction<Vector3[], bool> callback;

    public PathRequest(Vector3 _start, Vector3 _end, UnityAction<Vector3[], bool> _callback)
    {
        pathStart = _start;
        pathEnd = _end;
        callback = _callback;
    }
}
