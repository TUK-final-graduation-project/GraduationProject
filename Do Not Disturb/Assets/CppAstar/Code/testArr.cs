using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor.Callbacks;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct MyStruct
{
    [MarshalAs(UnmanagedType.I4)]
    public int nData;
    [MarshalAs(UnmanagedType.I1)]
    public bool bTrue;

    public MyStruct(int _nData, bool _bTrue)
    {
        this.nData = _nData;
        this.bTrue = _bTrue;
    }
}

public class testArr : MonoBehaviour
{
    [DllImport("testcpp")]
    public static extern int RecvGridInfo(MyStruct[] list, [MarshalAs(UnmanagedType.I4)] int nCount, [MarshalAs(UnmanagedType.I4)] int kCount, [MarshalAs(UnmanagedType.I4)] int index);

    [DllImport("testcpp")]
    public static extern void SendRoute(out IntPtr list, [MarshalAs(UnmanagedType.I4)] out int nCount);

    public static void GetGridInfo()
    {
        int nCount = 0;
        IntPtr info = IntPtr.Zero;
        // SendRoute(out info, out nCount);
        int nSize = Marshal.SizeOf(typeof(MyStruct));
        for (int nIndex = 0; nIndex < nCount; nIndex++)
        {
            MyStruct str = (MyStruct)Marshal.PtrToStructure(info, typeof(MyStruct));
            info = (IntPtr)(info.ToInt64() + nSize);
            Debug.Log(str.nData);
        }
        Debug.Log(nCount);
    }


    //private void Start()
    //{
    //    IList<MyStruct> myStructs = new List<MyStruct>();
    //    myStructs.Add(new MyStruct(100, false));
    //    myStructs.Add(new MyStruct(22, true));
    //    myStructs.Add(new MyStruct(13, false));
    //    myStructs.Add(new MyStruct(44, true));

    //    SimpleReturnFun(myStructs.ToArray(), 2, 2);

    //    int nCount = 0;
    //    IntPtr info;
    //    MyFunc(out info, out nCount);
    //    int nSize = Marshal.SizeOf(typeof(MyStruct));
    //    for (int nIndex = 0; nIndex < nCount; nIndex++)
    //    {
    //        MyStruct str = (MyStruct)Marshal.PtrToStructure(info, typeof(MyStruct));
    //        info = (IntPtr)(info.ToInt64() + nSize);
    //        Debug.Log(str.nData);
    //    }
    //    Debug.Log(nCount);
    //}
}
