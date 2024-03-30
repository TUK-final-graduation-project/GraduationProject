using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class VisualizeGrid : MonoBehaviour
{
    public LayerMask unWalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public NodeClass[,] grid;
    public Transform player;
    public Vector3 target;

    float nodeDiameter;
    int gridSizeX;
    int gridSizeY;

    NodeClass playerNode;
    NodeClass endNode;

    private void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
        CppBackend.SendWorldSizeInUnity(gridSizeX, gridSizeY);
        CreateGrid();
        SetTargetandPlayerNode(player.position, target);
        CppBackend.ReceivePathInUnity();
    }

    void CreateGrid()
    {
        grid = new NodeClass[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;
        Vector3 worldPoint;
        for(int x = 0; x < gridSizeX; x++)
        {
            for ( int y = 0; y < gridSizeY; y++)
            {
                worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius/2, unWalkableMask));
                grid[x, y] = new NodeClass(walkable, worldPoint);
                CppBackend.SendNodeInfoInUnity(x, y, walkable);
            }
        }
    }

    public void SetTargetandPlayerNode(Vector3 playerPosition, Vector3 targetPosition)
    {
        GetNodeFromWorldPoint(playerPosition);
        GetNodeFromWorldPointTarget(targetPosition);
    }
    // player, target의 위치 계산
    public void GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        CppBackend.SendStartNodeInUnity(x, y);

        for ( int i = 0; i < gridSizeX; i++ )
        {
            for ( int j = 0; j < gridSizeY; j++ )
            {
                if ( CppBackend.CompStartNodeInUnity(i, j))
                {
                    playerNode = grid[i, j];
                    Debug.Log(i+ " " + j);
                    break;
                }
            }
        }
    }

    public void GetNodeFromWorldPointTarget(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        CppBackend.SendEndNodeInUnity(x, y);

        for (int i = 0; i < gridSizeX; i++)
        {
            for (int j = 0; j < gridSizeY; j++)
            {
                if (CppBackend.CompEndNodeInUnity(i, j))
                {
                    endNode = grid[i, j];
                    Debug.Log(i + " " + j);
                    break;
                }
            }
        }
    }

    public NodeClass GetNodeFromWorldPointG(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if ( grid!=null)
        {
            //NodeClass playerNode = GetNodeFromWorldPointG(player.position);
            //NodeClass endNode = GetNodeFromWorldPointG(new Vector3(0, 0, 0));
            foreach (NodeClass node in grid)
            {
                Gizmos.color = (node.isWalkable) ? Color.green : Color.red;
                if (playerNode == node)
                {
                    Gizmos.color = Color.cyan;
                }
                if ( endNode == node)
                {
                    Gizmos.color = Color.black;
                }
                Gizmos.DrawCube(node.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        }
    }

}
