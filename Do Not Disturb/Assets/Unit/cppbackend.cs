using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class cppbackend : MonoBehaviour
{
    [DllImport("Test")]
    private static extern int SimpleTypeArgFun(int n);

    private void Start()
    {
        Run1_2();
    }
    private void Run1_2()
    {
        Debug.Log("1.2. Output: \t" + SimpleTypeArgFun(20));
    }
}