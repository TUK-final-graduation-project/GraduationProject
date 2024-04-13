using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public class NodeClass
{
    [MarshalAs(UnmanagedType.I1)]
    bool isWalkable;                     // �� �� �ִ� ������ ���� �Ǵ�
    [MarshalAs(UnmanagedType.I4)]
    int x;
    [MarshalAs(UnmanagedType.I4)]
    int y;// ����� ���� ��ǥ ����
    [MarshalAs(UnmanagedType.I4)]
    int z;// ����� ���� ��ǥ ����

    public NodeClass(bool n_isWalkable, int _x, int _y, int _z)
    {
        isWalkable = n_isWalkable;
        x = _x; 
        y = _y;
        z = _z;
    }
}
