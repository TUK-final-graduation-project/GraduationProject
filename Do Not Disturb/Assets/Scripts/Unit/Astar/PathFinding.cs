using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    AstarManager requestManager;
    public TestSceneManager testSceneManager;
    AGrid grid;

    // 시작 전 grid class 생성
    private void Awake()
    {
        requestManager = GetComponent<AstarManager>();
        grid = GetComponent<AGrid>();
    }

    // 시작
    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }

    // 길 찾기 (A*)
    IEnumerator FindPath(Vector3 StartPos, Vector3 TargetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node StartNode = grid.GetNodeFromWorldPoint(StartPos);
        Node TargetNode = grid.GetNodeFromWorldPoint(TargetPos);

        if ( StartNode.isWalkable && TargetNode.isWalkable )
        {
            List<Node> OpenList = new List<Node>();
            HashSet<Node> CloseList = new HashSet<Node>();
            OpenList.Add(StartNode);

            while(OpenList.Count > 0)
            {
                Node currentNode = OpenList[0];

                for (int i = 1;i < OpenList.Count; i++)
                {
                    if (OpenList[i].fCost < currentNode.fCost || OpenList[i].fCost == currentNode.fCost && OpenList[i].hCost < currentNode.hCost) 
                    { 
                        currentNode = OpenList[i];
                    }
                }

                OpenList.Remove(currentNode);
                CloseList.Add(currentNode);

                if (currentNode == TargetNode)
                {
                    pathSuccess = true;
                    break;
                }
                foreach (Node n in grid.GetNeighbours(currentNode))
                {
                    if (!n.isWalkable || CloseList.Contains(n)) continue;

                    int newCurrentToNeighbourCost = currentNode.gCost + GetDistanceCost(currentNode, n);
                    if (newCurrentToNeighbourCost < n.gCost || !OpenList.Contains(n))
                    {
                        n.gCost = newCurrentToNeighbourCost;
                        n.hCost = GetDistanceCost(n, TargetNode);
                        n.parentNode = currentNode;

                        if (!OpenList.Contains(n))
                        {
                            OpenList.Add(n);
                        }
                    }

                }
            }

        }
        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(StartNode, TargetNode);
        }

        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    // close list의 노드 길로 만들기
    Vector3[] RetracePath(Node StartNode, Node EndNode)
    {

        Debug.Log("start:" + StartNode.worldPosition);
        Debug.Log("end:" + EndNode.worldPosition);
        List<Node> path = new List<Node>();
        Node currentNode = EndNode;

        while (currentNode != StartNode) 
        { 
            path.Add(currentNode);
            currentNode = currentNode.parentNode;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        grid.path = path;
        return waypoints;
    }

    // 길 정리하기
    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        waypoints.Add(path[0].worldPosition);
        for (int i = 2; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX, path[i-1].gridY - path[i].gridY);
            if (directionOld != directionNew)
            {
                waypoints.Add(path[i-1].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    // cost 계산하기
    int GetDistanceCost(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        // 가로, 세로 한 칸: 10 || 대각선: 14로 cost 계산함
        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        return 14 * distX + 10 * (distY - distX);
    }
}
