using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AGrid : MonoBehaviour
{
    public LayerMask unWalkAbleMask;
    public Vector2 GridWorldSize;
    public float NodeRadius;
    Node[,] grid;

    float NodeDiameter;
    int GridSizeX;
    int GridSizeY;

    // Start is called before the first frame update
    void Start()
    {
        NodeDiameter = NodeRadius * 2;
        GridSizeX = Mathf.RoundToInt(GridWorldSize.x/ NodeDiameter);
        GridSizeY = Mathf.RoundToInt(GridWorldSize.y/ NodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[GridSizeX, GridSizeY];
        Vector3 WorldBottonLeft = transform.position - Vector3.right * GridWorldSize.x / 2 - Vector3.forward * GridWorldSize.y / 2;
        Vector3 WorldPoint;

        for (int x = 0; x < GridSizeX; x++)
        {
            for ( int y = 0; y < GridSizeY; y++)
            {
                WorldPoint = WorldBottonLeft + Vector3.right * (x * NodeDiameter + NodeRadius) + Vector3.forward * (y * NodeDiameter + NodeRadius);
                bool WalkAble = !(Physics.CheckSphere(WorldPoint, NodeRadius, unWalkAbleMask));
                grid[x, y] = new Node(WalkAble, WorldPoint);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(GridWorldSize.x, 1, GridWorldSize.y));
        if (grid != null)
        {
            foreach (Node node in grid)
            {
                Gizmos.color = (node.isWalkAble) ? Color.white : Color.red;
                Gizmos.DrawCube(node.WorldPos, Vector3.one * (NodeDiameter - .1f));
            }
        }
    }
}
