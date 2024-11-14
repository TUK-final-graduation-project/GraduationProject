using System.Collections.Generic;
using UnityEngine;

public class MeshSlice
{
    public Mesh originMesh;

    public MeshSlice(Mesh mesh)
    {
        originMesh = mesh;
    }

    // 메쉬를 슬라이스하고 결과를 반환
    public (Mesh, Mesh) Slice(SlicePlane plane)
    {
        List<Vector3> vertexTop = new List<Vector3>();
        List<Vector3> vertexBottom = new List<Vector3>();

        // 슬라이스할 삼각형을 검사
        for (int i = 0; i < originMesh.triangles.Length; i += 3)
        {
            Vector3 v1 = originMesh.vertices[originMesh.triangles[i]];
            Vector3 v2 = originMesh.vertices[originMesh.triangles[i + 1]];
            Vector3 v3 = originMesh.vertices[originMesh.triangles[i + 2]];

            // 삼각형의 꼭짓점 위치에 따라 분류
            ClassifyTriangle(v1, v2, v3, plane, vertexTop, vertexBottom);
        }

        // 상단, 하단 메쉬 생성 및 반환
        Mesh meshTop = MeshManipulator.CreateMesh(vertexTop);
        Mesh meshBottom = MeshManipulator.CreateMesh(vertexBottom);

        return (meshTop, meshBottom);
    }

    private void ClassifyTriangle(Vector3 v1, Vector3 v2, Vector3 v3, SlicePlane plane, List<Vector3> vertexTop, List<Vector3> vertexBottom)
    {
        // 각 꼭짓점의 위치에 따라 슬라이스 삼각형을 분류
        bool v1Top = plane.IsAbove(v1);
        bool v2Top = plane.IsAbove(v2);
        bool v3Top = plane.IsAbove(v3);

        if (v1Top && v2Top && v3Top)
        {
            vertexTop.AddRange(new[] { v1, v2, v3 });
        }
        else if (!v1Top && !v2Top && !v3Top)
        {
            vertexBottom.AddRange(new[] { v1, v2, v3 });
        }
        else
        {
            // 꼭짓점이 혼합된 경우 슬라이스 계산
            SliceTriangle(v1, v2, v3, v1Top, v2Top, v3Top, vertexTop, vertexBottom, plane);
        }
    }

    private void SliceTriangle(Vector3 v1, Vector3 v2, Vector3 v3, bool v1Top, bool v2Top, bool v3Top, List<Vector3> vertexTop, List<Vector3> vertexBottom, SlicePlane plane)
    {
        // 여기서 슬라이스 로직 구현 (각각의 꼭짓점을 기준으로 교차점을 찾고 새 꼭짓점 추가)
        Vector3 intersection1 = plane.Intersect(v1, v2);
        Vector3 intersection2 = plane.Intersect(v2, v3);

        if (v1Top)
        {
            vertexTop.Add(v1);
            vertexTop.Add(intersection1);
            vertexTop.Add(intersection2);
            vertexBottom.Add(intersection1);
            vertexBottom.Add(v2);
            vertexBottom.Add(v3);
        }
        else
        {
            vertexBottom.Add(v1);
            vertexBottom.Add(intersection1);
            vertexBottom.Add(intersection2);
            vertexTop.Add(intersection1);
            vertexTop.Add(v2);
            vertexTop.Add(v3);
        }
    }
}

public class SlicePlane
{
    private Vector3 point;
    private Vector3 normal;

    public SlicePlane(Vector3 point, Vector3 normal)
    {
        this.point = point;
        this.normal = normal;
    }

    // 점이 평면 위에 있는지 여부를 확인
    public bool IsAbove(Vector3 vertex)
    {
        return Vector3.Dot(vertex - point, normal) >= 0;
    }

    // 두 점 사이의 교차점 계산
    public Vector3 Intersect(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        float t = Vector3.Dot(point - start, normal) / Vector3.Dot(direction, normal);
        return start + t * direction;
    }
}

public static class MeshManipulator
{
    // 주어진 정점 목록으로 메쉬 생성
    public static Mesh CreateMesh(List<Vector3> vertex)
    {
        Mesh mesh = new Mesh();
        mesh.SetVertices(vertex);

        // 삼각형 배열 및 기타 정보 설정
        List<int> triangles = GenerateTriangles(vertex.Count);
        mesh.SetTriangles(triangles, 0);

        mesh.RecalculateNormals();
        return mesh;
    }

    // 삼각형 인덱스 생성
    private static List<int> GenerateTriangles(int vertexCount)
    {
        List<int> triangles = new List<int>();
        for (int i = 0; i < vertexCount; i += 3)
        {
            triangles.Add(i);
            triangles.Add(i + 1);
            triangles.Add(i + 2);
        }
        return triangles;
    }
}
