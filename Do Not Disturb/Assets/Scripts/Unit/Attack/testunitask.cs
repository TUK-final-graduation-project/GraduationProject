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
        //StartCoroutine(wait1Second());  //�ڷ�ƾ
        wait1Second().Forget();   //�����½�ũ
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
