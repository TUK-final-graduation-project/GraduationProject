using UnityEngine;

public class Sliceable : MonoBehaviour
{
    [SerializeField]
    private bool isClosedMesh = true;

    [SerializeField]
    private bool shareVertex = false;

    [SerializeField]
    private bool smoothVertex = false;

    [SerializeField]
    private bool reverseWindTriangle = false;

    // Auto-properties 활용하여 코드 간소화
    public bool IsClosedMesh
    {
        get => isClosedMesh;
        set => isClosedMesh = value;
    }

    public bool ShareVertex
    {
        get => shareVertex;
        set => shareVertex = value;
    }

    public bool SmoothVertex
    {
        get => smoothVertex;
        set => smoothVertex = value;
    }

    public bool ReverseWindTriangle
    {
        get => reverseWindTriangle;
        set => reverseWindTriangle = value;
    }
}
