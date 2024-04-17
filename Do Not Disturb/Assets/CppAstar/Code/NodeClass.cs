using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class NodeClass
{
    public bool isWalkable;
    public Vector3 worldPos;

    public NodeClass(bool n_isWalkable, Vector3 _worldPos)
    {
        isWalkable = n_isWalkable;
        worldPos = _worldPos;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SendNode
{
    [MarshalAs(UnmanagedType.I4)]
    public int x;
    [MarshalAs(UnmanagedType.I4)]
    public int y;
    [MarshalAs(UnmanagedType.I1)]
    bool isWalkable;

    public SendNode(int _x, int _y, bool _isWalkable)
    {
        this.x = _x;
        this.y = _y;
        this.isWalkable = _isWalkable;
    }
}