using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool isWalkAble;
    public Vector3 WorldPos;

    public Node(bool isWalkAble, Vector3 worldPos)
    {
        this.isWalkAble = isWalkAble;
        WorldPos = worldPos;
    }
}
