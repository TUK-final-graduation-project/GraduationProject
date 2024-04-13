using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public struct MyStruct
{
    [MarshalAs(UnmanagedType.I4)]
    int nData;
    [MarshalAs(UnmanagedType.I1)]
    bool bTrue;

    public MyStruct(int _nData, bool _bTrue)
    {
        this.nData = _nData;
        this.bTrue = _bTrue;
    }
}
public class testArr : MonoBehaviour
{
    [DllImport("testcpp")]
    private static extern float SimpleReturnFun(MyStruct[] list, [MarshalAs(UnmanagedType.I4)] int nCount);


    private void Start()
    {
        IList<MyStruct> objList = new List<MyStruct>();
        objList.Add(new MyStruct(20, false));
        objList.Add(new MyStruct(11, false));

        Debug.Log(SimpleReturnFun(objList.ToArray(), objList.Count));
    }

}
