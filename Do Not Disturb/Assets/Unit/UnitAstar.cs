using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class UnitAstar : MonoBehaviour
{
    [DllImport("Test")]
    private static extern int SimpleTypeArgFun(int n);

    // Start is called before the first frame update
    private void Start()
    {
        Run1_2();
    }
    private void Run1_2()
    {
        Debug.Log("1.2. Output: \t" + SimpleTypeArgFun(20));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
