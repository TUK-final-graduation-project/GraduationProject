using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeClass
{
    public bool isWalkable;                     // 갈 수 있는 곳인지 여부 판단
    public Vector3 worldPosition;               // 노드의 월드 좌표 정보

    public NodeClass(bool n_isWalkable, Vector3 n_WorldPos)
    {
        isWalkable = n_isWalkable;
        worldPosition = n_WorldPos;
    }
}
