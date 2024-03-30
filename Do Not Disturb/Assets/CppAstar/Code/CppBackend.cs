using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CppBackend : MonoBehaviour
{
    [DllImport("backend")]
    private static extern IntPtr SimpleReturnFun(float x, float y, bool isWalkable);

    [DllImport("backend")]
    private static extern int Re(int[] n, int size);

    [DllImport("backend")]
    private static extern int SendWorldSize(int x, int y);

    [DllImport("backend")]
    private static extern bool SendNodeInfo(int x, int y, bool isWalkable);

    [DllImport("backend")]
    private static extern int SendStartNode(int x, int y);

    [DllImport("backend")]
    private static extern int SendEndNode(int x, int y);

    [DllImport("backend")]
    private static extern bool CompStartNode(int x, int y);

    [DllImport("backend")]
    private static extern bool CompEndNode(int x, int y);

    [DllImport("backend")]
    private static extern int ReceivePath();

    public static void SimpleReturn(float x, float y, bool isWalkable)
    {
        string s = Marshal.PtrToStringAnsi(SimpleReturnFun(x, y, isWalkable));
        Debug.Log(s);
    }

    public static void SendWorldSizeInUnity(int x, int y)
    {
        Debug.Log(SendWorldSize(x, y));
    }

    public static void SendNodeInfoInUnity(int x, int y, bool isWalkable)
    {
        Debug.Log(SendNodeInfo(x, y, isWalkable));
    }

    public static void SendStartNodeInUnity(int x, int y)
    {
        Debug.Log(SendStartNode(x, y));
    }

    public static void SendEndNodeInUnity(int x, int y)
    {
        Debug.Log(SendEndNode(x, y));
    }
    
    public static bool CompStartNodeInUnity(int x, int y)
    {
        return CompStartNode(x, y);
    }

    public static bool CompEndNodeInUnity(int x, int y)
    {
        return CompEndNode(x, y);
    }

    public static void ReceivePathInUnity()
    {
        Debug.Log(ReceivePath());
    }
}
