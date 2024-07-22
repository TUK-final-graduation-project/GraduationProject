using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class testunitask : MonoBehaviour
{
    int i;
    private void Start()
    {
        //StartCoroutine(wait1Second());  //코루틴
        wait1Second().Forget();   //유니태스크
    }

    private async UniTaskVoid wait1Second()
    {
        while (true)
        { 
            Debug.Log(i);
            ++i;
            await UniTask.Delay(1000);        
        }
    }

}
