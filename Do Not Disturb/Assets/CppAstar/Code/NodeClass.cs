using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public class NodeClass
{
    [MarshalAs(UnmanagedType.I1)]
    bool isWalkable;                     // 갈 수 있는 곳인지 여부 판단
    [MarshalAs(UnmanagedType.I4)]
    int x;
    [MarshalAs(UnmanagedType.I4)]
    int y;// 노드의 월드 좌표 정보
    [MarshalAs(UnmanagedType.I4)]
    int z;// 노드의 월드 좌표 정보

    public NodeClass(bool n_isWalkable, int _x, int _y, int _z)
    {
        isWalkable = n_isWalkable;
        x = _x; 
        y = _y;
        z = _z;
    }
}
