using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential)]
public class TestC
{
    public bool isWalkable;
    public float x;

    public TestC(bool n_isWalkable, float n_WorldPos)
    {
        isWalkable = n_isWalkable;
        x = n_WorldPos;
    }
}
public class testArr : MonoBehaviour
{
    [DllImport("testcpp")]
    private static extern float SimpleReturnFun(TestC t);

    public TestC testclass = new TestC(false, 3f);

    private void Start()
    {
        Debug.Log(SimpleReturnFun(testclass));
    }

}
