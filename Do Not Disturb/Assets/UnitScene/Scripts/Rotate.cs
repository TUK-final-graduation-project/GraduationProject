using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public enum Type { up, right, left };
    public Type type;
    public int degree = 30;

    public bool isRotate = true;
    Vector3 dir;
    private void Start()
    {
        switch (type)
        {
            case Type.up:
                dir = new Vector3(0, 1, 0); 
                break;
            case Type.right: 
                dir = Vector3.right; 
                break;
        }
    }
    void Update()
    {
        if(isRotate)
        {
            transform.Rotate(dir * Time.deltaTime * degree);
        }
    }
}
