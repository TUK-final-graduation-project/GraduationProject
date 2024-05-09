using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCubeScript : MonoBehaviour
{
    public AGrid agrid;

    Vector2 gridWorldSize;
    Node[,] grid;
    float nodeDiameter;

    private void Start()
    {
        gridWorldSize = agrid.gridWorldSize;
        grid = agrid.grid;
        nodeDiameter = agrid.nodeDiameter;

    }

}
