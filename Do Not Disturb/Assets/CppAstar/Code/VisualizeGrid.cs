using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UIElements;

public class VisualizeGrid : MonoBehaviour
{
    [DllImport("testcpp")]
    public static extern int RecvGridInfo(SendNode[] list, [MarshalAs(UnmanagedType.I4)] int nCount, [MarshalAs(UnmanagedType.I4)] int kCount, [MarshalAs(UnmanagedType.I4)] int index);

    [DllImport("testcpp")]
    public static extern void SendRoute(out IntPtr list, [MarshalAs(UnmanagedType.I4)] out int nCount);

    [DllImport("testcpp")]
    public static extern int SendPlayerTarget([MarshalAs(UnmanagedType.I4)] int playerX, [MarshalAs(UnmanagedType.I4)] int playerY, [MarshalAs(UnmanagedType.I4)] int targetX, [MarshalAs(UnmanagedType.I4)] int targetY);

    public LayerMask unWalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public NodeClass[,] grid;
    public SendNode[] gridNodes;
    float nodeDiameter;
    int gridSizeX;
    int gridSizeY;

    public Transform player;
    public Transform target;

    private void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
        CreateGrid();
        Vector2 playerNode = GetNodeFromWorldPoint(player.position);
        Vector2 targetNode = GetNodeFromWorldPoint(target.position);
        // Debug.Log(SendPlayerTarget((int)playerNode.x, (int)playerNode.y, (int)targetNode.x, (int)targetNode.y));
    }

    void CreateGrid()
    {
        grid = new NodeClass[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;
        Vector3 worldPoint;
        gridNodes = new SendNode[gridSizeX*gridSizeY];
        int i = 0;
        for(int x = 0; x < gridSizeX; x++)
        {
            for ( int y = 0; y < gridSizeY; y++)
            {
                worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius/2, unWalkableMask));
                grid[x, y] = new NodeClass(walkable, worldPoint);
                gridNodes[i] = new SendNode(x,y, walkable);
                ++i;
            }
        }

        Debug.Log("¿¨? " + RecvGridInfo(gridNodes.ToArray(), gridSizeX, gridSizeY, 0));

    }
    public Vector2 GetNodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return new Vector2(x, y);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null)
        {
            foreach (NodeClass node in grid)
            {
                if (!node.isWalkable)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawCube(node.worldPos, Vector3.one * (nodeDiameter - .1f));
                }
            }
        }
    }

}
