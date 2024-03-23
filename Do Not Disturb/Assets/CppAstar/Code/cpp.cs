using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class cpp : MonoBehaviour
{
    [DllImport("astar")]
    private static extern int SimpleReturnFun();
}
