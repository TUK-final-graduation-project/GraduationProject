using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;

public class Sliceable : MonoBehaviour
{
    [SerializeField]
    private bool isSolid = true;

    [SerializeField]
    private bool reverseWindTriangles = false;

    [SerializeField]
    private bool useGravity = true;

    [SerializeField]
    private bool shareVertices = false;

    [SerializeField]
    private bool smoothVertices = false;

    public bool IsSolid
    {
        get
        {
            return isSolid;
        }
        set
        {
            isSolid = value;
        }
    }

    public bool ReverseWireTriangles
    {
        get
        {
            return reverseWindTriangles;
        }
        set
        {
            reverseWindTriangles = value;
        }
    }

    public bool UseGravity
    {
        get
        {
            return useGravity;
        }
        set
        {
            useGravity = value;
        }
    }

    public bool ShareVertices
    {
        get
        {
            return shareVertices;
        }
        set
        {
            shareVertices = value;
        }
    }

    public bool SmoothVertices
    {
        get
        {
            return smoothVertices;
        }
        set
        {
            smoothVertices = value;
        }
    }
}