using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int x;
    public int z;

    public Node parent;

    private int F;
    private int G;
    private int H;

    public Node(int _x, int _z)
    {
        x = _x;
        z = _z;
    }
}
