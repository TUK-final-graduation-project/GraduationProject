using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CreateGrid : MonoBehaviour
{
    // grid 정보 전송
    [DllImport("astar")]
    public static extern void GridInfo(SendNode[] list, [MarshalAs(UnmanagedType.I4)] int xSize, [MarshalAs(UnmanagedType.I4)] int ySize);

    public LayerMask unWalkableMask;
    public Vector2 gridWorldSize;
    public Node

    float nodeRadius = 0.5f;
}
