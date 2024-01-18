using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class cppbackend : MonoBehaviour
{
    [DllImport("backend")]
    private static extern int SimpleReturnFun();

    private void Start()
    {
        Debug.Log(SimpleReturnFun());
    }
}
