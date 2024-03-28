using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CppBackend : MonoBehaviour
{
    [DllImport("backend")]

    private static extern IntPtr SimpleReturnFun(float x, float y, bool isWalkable);

    public static void SimpleReturn(float x, float y, bool isWalkable)
    {
        string s = Marshal.PtrToStringAnsi(SimpleReturnFun(x, y, isWalkable));
        Debug.Log(s);
    }
}
