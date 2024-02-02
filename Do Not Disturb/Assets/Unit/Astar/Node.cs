using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Node
{
    public bool isWalkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Node parentNode;

    public Node(bool _isWalkable, Vector3 _worldPos, int nGridX, int nGridY)
    {
        isWalkable = _isWalkable;
        worldPosition = _worldPos;
        gridX = nGridX;
        gridY = nGridY;
    }
    public int fCost
    {
        get { return gCost + hCost; }
    }
}
