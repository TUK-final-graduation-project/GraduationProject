using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testArr : MonoBehaviour
{
    int[,] arr;
    // Start is called before the first frame update
    void Start()
    {
        int k = 0;
        arr = new int[3,3];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                arr[i,j] = k;
                k++;
            }
        }
        for ( int i =0; i < 3; i++)
        {
            Debug.Log(arr[1, i]);
        }
    }
}
