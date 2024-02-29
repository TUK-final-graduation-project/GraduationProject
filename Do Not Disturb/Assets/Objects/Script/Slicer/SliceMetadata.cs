using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using UnityEngine.UIElements;
namespace Assets.Scripts
{
    /// <summary>
    /// �޽��� ������ ��Ÿ���� ������
    /// </summary>
    public enum MeshSide
    {
        Positive = 0,
        Negative = 1
    }

    /// <summary>
    /// �����̽��� ��ü�� ���� �� �޽� �����͸� �����ϴ� ��ü
    /// </summary>
    class SlicesMetadata
    {
        private Mesh positiveSideMesh;
        private List<Vector3> positiveSideVertices;
        private List<int> positiveSideTriangles;
        private List<Vector2> positiveSideUvs;
        private List<Vector3> positiveSideNormals;

        private Mesh negativeSideMesh;
        private List<Vector3> negativeSideVertices;
        private List<int> negativeSideTriangles;
        private List<Vector2> negativeSideUvs;
        private List<Vector3> negativeSideNormals;

        private readonly List<Vector3> pointsAlongPlane;
        private Plane plane;
        private Mesh mesh;
        private bool isSolid;
        private bool useSharedVertices = false;
        private bool smoothVertices = false;
        private bool createReverseTriangleWindings = false;


        // ��ü�� ��ü���� ���θ� ��Ÿ���� ������Ƽ
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

        // ��� �� �޽ø� �������� ������Ƽ
        public Mesh PositiveSideMesh
        {
            get
            {
                if (positiveSideMesh == null)
                {
                    positiveSideMesh = new Mesh();
                }

                SetMeshData(MeshSide.Positive);
                return positiveSideMesh;
            }
        }

        // ���� �� �޽ø� �������� ������Ƽ
        public Mesh NegativeSideMesh
        {
            get
            {
                if (negativeSideMesh == null)
                {
                    negativeSideMesh = new Mesh();
                }

                SetMeshData(MeshSide.Negative);

                return negativeSideMesh;
            }
        }


        // �����̽��� ��ü�� �ʱ�ȭ �� �޽� ����� ����ϴ� ������

        /// <param name="plane">�����̽� ���</param>
        /// <param name="mesh">���� �޽�</param>
        /// <param name="isSolid">��ü���� ����</param>
        /// <param name="createReverseTriangleWindings">�� �ﰢ�� Ʈ���̾ޱ��� ������ ����</param>
        /// <param name="shareVertices">������ ���� ��� ����</param>
        /// <param name="smoothVertices">������ �ε巴�� ó������ ����</param>

        public SlicesMetadata(Plane plane, Mesh mesh, bool isSolid, bool createReverseTriangleWindings, bool shareVertices, bool smoothVertices)
        {
            positiveSideTriangles = new List<int>();
            positiveSideVertices = new List<Vector3>();
            negativeSideTriangles = new List<int>();
            negativeSideVertices = new List<Vector3>();
            positiveSideUvs = new List<Vector2>();
            negativeSideUvs = new List<Vector2>();
            positiveSideNormals = new List<Vector3>();
            negativeSideNormals = new List<Vector3>();
            pointsAlongPlane = new List<Vector3>();
            this.plane = plane;
            this.mesh = mesh;
            this.isSolid = isSolid;
            this.createReverseTriangleWindings = createReverseTriangleWindings;
            useSharedVertices = shareVertices;
            this.smoothVertices = smoothVertices;

            ComputeNewMeshes();
        }

        // �޽� �����͸� �ùٸ� �鿡 �߰��ϰ� ���� ���͸� ����ϴ� �޼ҵ�

        /// <param name="side">�߰��� ��</param>
        /// <param name="vertex1">ù ��° ����</param>
        /// <param name="normal1">ù ��° ���� ���� ����</param>
        /// <param name="uv1">ù ��° ���� UV ��ǥ</param>
        /// <param name="vertex2">�� ��° ����</param>
        /// <param name="normal2">�� ��° ���� ���� ����</param>
        /// <param name="uv2">�� ��° ���� UV ��ǥ</param>
        /// <param name="vertex3">�� ��° ����</param>
        /// <param name="normal3">�� ��° ���� ���� ����</param>
        /// <param name="uv3">�� ��° ���� UV ��ǥ</param>
        /// <param name="shareVertices">������ �������� ����</param>
        /// <param name="addFirst">ù ��° ������ ���� �߰����� ����</param>
        private void AddTrianglesNormalAndUvs(MeshSide side, Vector3 vertex1, Vector3? normal1, Vector2 uv1, Vector3 vertex2, Vector3? normal2, Vector2 uv2, Vector3 vertex3, Vector3? normal3, Vector2 uv3, bool shareVertices, bool addFirst)
        {
            if (side == MeshSide.Positive)
            {
                AddTrianglesNormalsAndUvs(ref positiveSideVertices, ref positiveSideTriangles, ref positiveSideNormals, ref positiveSideUvs, vertex1, normal1, uv1, vertex2, normal2, uv2, vertex3, normal3, uv3, shareVertices, addFirst);
            }
            else
            {
                AddTrianglesNormalsAndUvs(ref negativeSideVertices, ref negativeSideTriangles, ref negativeSideNormals, ref negativeSideUvs, vertex1, normal1, uv1, vertex2, normal2, uv2, vertex3, normal3, uv3, shareVertices, addFirst);
            }
        }

        // ������ �޽ÿ� �߰��ϰ� ���� ������ ���� �ﰢ���� �����մϴ�.
        // ������ ������ �ƴ� ��� ��ġ�ϴ� ������ �̹� �����ϴ��� ������ �߰��˴ϴ�.
        // ���� ���͸� ������� �ʽ��ϴ�.

        /// <param name="vertices">���� ����Ʈ</param>
        /// <param name="triangles">�ﰢ�� ����Ʈ</param>
        /// <param name="uvs">UV ��ǥ ����Ʈ</param>
        /// <param name="normals">���� ���� ����Ʈ</param>
        /// <param name="vertex1">ù ��° ����</param>
        /// <param name="normal1">ù ��° ���� ���� ����</param>
        /// <param name="uv1">ù ��° ���� UV ��ǥ</param>
        /// <param name="vertex2">�� ��° ����</param>
        /// <param name="normal2">�� ��° ���� ���� ����</param>
        /// <param name="uv2">�� ��° ���� UV ��ǥ</param>
        /// <param name="vertex3">�� ��° ����</param>
        /// <param name="normal3">�� ��° ���� ���� ����</param>
        /// <param name="uv3">�� ��° ���� UV ��ǥ</param>
        /// <param name="shareVertices">������ �������� ����</param>
        /// <param name="addFirst">ù ��° ������ ���� �߰����� ����</param>
        private void AddTrianglesNormalsAndUvs(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector3> normals, ref List<Vector2> uvs, Vector3 vertex1, Vector3? normal1, Vector2 uv1, Vector3 vertex2, Vector3? normal2, Vector2 uv2, Vector3 vertex3, Vector3? normal3, Vector2 uv3, bool shareVertices, bool addFirst)
        {
            int tri1Index = vertices.IndexOf(vertex1);

            if (addFirst)
            {
                ShiftTriangleIndeces(ref triangles);
            }

            // ������ �̹� �����ϴ� ���, ������ ���� �ﰢ�� ������ �߰��ϰ� �׷��� ������ ������ ����Ʈ�� �߰��ϰ� �ﰢ�� �ε����� �߰��մϴ�.
            if (tri1Index > -1 && shareVertices)
            {
                triangles.Add(tri1Index);
            }
            else
            {
                if (normal1 == null)
                {
                    normal1 = ComputeNormal(vertex1, vertex2, vertex3);
                }

                int? i = null;
                if (addFirst)
                {
                    i = 0;
                }

                AddVertNormalUv(ref vertices, ref normals, ref uvs, ref triangles, vertex1, (Vector3)normal1, uv1, i);
            }

            int tri2Index = vertices.IndexOf(vertex2);

            if (tri2Index > -1 && shareVertices)
            {
                triangles.Add(tri2Index);
            }
            else
            {
                if (normal2 == null)
                {
                    normal2 = ComputeNormal(vertex2, vertex3, vertex1);
                }

                int? i = null;

                if (addFirst)
                {
                    i = 1;
                }

                AddVertNormalUv(ref vertices, ref normals, ref uvs, ref triangles, vertex2, (Vector3)normal2, uv2, i);
            }

            int tri3Index = vertices.IndexOf(vertex3);

            if (tri3Index > -1 && shareVertices)
            {
                triangles.Add(tri3Index);
            }
            else
            {
                if (normal3 == null)
                {
                    normal3 = ComputeNormal(vertex3, vertex1, vertex2);
                }

                int? i = null;
                if (addFirst)
                {
                    i = 2;
                }

                AddVertNormalUv(ref vertices, ref normals, ref uvs, ref triangles, vertex3, (Vector3)normal3, uv3, i);
            }
        }


        // ����, ���� ����, UV ��ǥ �� �ﰢ���� �߰��ϴ� �޼ҵ�

        /// <param name="vertices">���� ����Ʈ</param>
        /// <param name="normals">���� ���� ����Ʈ</param>
        /// <param name="uvs">UV ��ǥ ����Ʈ</param>
        /// <param name="triangles">�ﰢ�� ����Ʈ</param>
        /// <param name="vertex">����</param>
        /// <param name="normal">���� ����</param>
        /// <param name="uv">UV ��ǥ</param>
        /// <param name="index">������ �ε���</param>
        private void AddVertNormalUv(ref List<Vector3> vertices, ref List<Vector3> normals, ref List<Vector2> uvs, ref List<int> triangles, Vector3 vertex, Vector3 normal, Vector2 uv, int? index)
        {
            if (index != null)
            {
                int i = (int)index;
                vertices.Insert(i, vertex);
                uvs.Insert(i, uv);
                normals.Insert(i, normal);
                triangles.Insert(i, i);
            }
            else
            {
                vertices.Add(vertex);
                normals.Add(normal);
                uvs.Add(uv);
                triangles.Add(vertices.IndexOf(vertex));
            }
        }

        // �ﰢ�� �ε����� �����Ͽ� �޽ø� ��ȯ�ϴ� �޼ҵ�

        /// <param name="triangles">�ﰢ�� ����Ʈ</param>
        private void ShiftTriangleIndeces(ref List<int> triangles)
        {
            for (int j = 0; j < triangles.Count; j += 3)
            {
                triangles[j] += +3;
                triangles[j + 1] += 3;
                triangles[j + 2] += 3;
            }
        }

        // ��ü�� ���θ� �������ϴ� �޼ҵ�
        // ��� ������ �����ϰ� �� �ﰢ�� ������ �����Ͽ� ����� ���� �� �� �ִ�.
        private void AddReverseTriangleWinding()
        {
            int positiveVertsStartIndex = positiveSideVertices.Count;
            // ���� ���� ����
            positiveSideVertices.AddRange(positiveSideVertices);
            positiveSideUvs.AddRange(positiveSideUvs);
            positiveSideNormals.AddRange(FlipNormals(positiveSideNormals));

            int numPositiveTriangles = positiveSideTriangles.Count;

            // �� ���� �ﰢ�� �߰�
            for (int i = 0; i < numPositiveTriangles; i += 3)
            {
                positiveSideTriangles.Add(positiveVertsStartIndex + positiveSideTriangles[i]);
                positiveSideTriangles.Add(positiveVertsStartIndex + positiveSideTriangles[i + 2]);
                positiveSideTriangles.Add(positiveVertsStartIndex + positiveSideTriangles[i + 1]);
            }

            int negativeVertextStartIndex = negativeSideVertices.Count;
            // ���� ���� ����
            negativeSideVertices.AddRange(negativeSideVertices);
            negativeSideUvs.AddRange(negativeSideUvs);
            negativeSideNormals.AddRange(FlipNormals(negativeSideNormals));

            int numNegativeTriangles = negativeSideTriangles.Count;

            // �� ���� �ﰢ�� �߰�
            for (int i = 0; i < numNegativeTriangles; i += 3)
            {
                negativeSideTriangles.Add(negativeVertextStartIndex + negativeSideTriangles[i]);
                negativeSideTriangles.Add(negativeVertextStartIndex + negativeSideTriangles[i + 2]);
                negativeSideTriangles.Add(negativeVertextStartIndex + negativeSideTriangles[i + 1]);
            }
        }


        /// <summary>
        /// ����� ���� ������ �߰� ������ �����ϴ� �޼ҵ�
        /// </summary>
        private void JoinPointsAlongPlane()
        {
            Vector3 halfway = GetHalfwayPoint(out float distance);

            for (int i = 0; i < pointsAlongPlane.Count; i += 2)
            {
                Vector3 firstVertex;
                Vector3 secondVertex;

                firstVertex = pointsAlongPlane[i];
                secondVertex = pointsAlongPlane[i + 1];

                Vector3 normal3 = ComputeNormal(halfway, secondVertex, firstVertex);
                normal3.Normalize();

                var direction = Vector3.Dot(normal3, plane.normal);

                if (direction > 0)
                {
                    AddTrianglesNormalAndUvs(MeshSide.Positive, halfway, -normal3, Vector2.zero, firstVertex, -normal3, Vector2.zero, secondVertex, -normal3, Vector2.zero, false, true);
                    AddTrianglesNormalAndUvs(MeshSide.Negative, halfway, normal3, Vector2.zero, secondVertex, normal3, Vector2.zero, firstVertex, normal3, Vector2.zero, false, true);
                }
                else
                {
                    AddTrianglesNormalAndUvs(MeshSide.Positive, halfway, normal3, Vector2.zero, secondVertex, normal3, Vector2.zero, firstVertex, normal3, Vector2.zero, false, true);
                    AddTrianglesNormalAndUvs(MeshSide.Negative, halfway, -normal3, Vector2.zero, firstVertex, -normal3, Vector2.zero, secondVertex, -normal3, Vector2.zero, false, true);
                }
            }
        }

        /// <summary>
        /// ù ��° ���� ���� �ָ� ������ �� ������ �߰� ������ ���ϴ� �޼ҵ�
        /// </summary>
        /// <param name="distance">���� �� ������ �Ÿ�</param>
        /// <returns>�߰� ����</returns>
        private Vector3 GetHalfwayPoint(out float distance)
        {
            if (pointsAlongPlane.Count > 0)
            {
                Vector3 firstPoint = pointsAlongPlane[0];
                Vector3 furthestPoint = Vector3.zero;
                distance = 0f;

                foreach (Vector3 point in pointsAlongPlane)
                {
                    float currentDistance = 0f;
                    currentDistance = Vector3.Distance(firstPoint, point);

                    if (currentDistance > distance)
                    {
                        distance = currentDistance;
                        furthestPoint = point;
                    }
                }

                return Vector3.Lerp(firstPoint, furthestPoint, 0.5f);
            }
            else
            {
                distance = 0;
                return Vector3.zero;
            }
        }

        /// <summary>
        /// �޽� �����͸� �����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="side">�޽��� ��</param>
        private void SetMeshData(MeshSide side)
        {
            if (side == MeshSide.Positive)
            {
                positiveSideMesh.vertices = positiveSideVertices.ToArray();
                positiveSideMesh.triangles = positiveSideTriangles.ToArray();
                positiveSideMesh.normals = positiveSideNormals.ToArray();
                positiveSideMesh.uv = positiveSideUvs.ToArray();
            }
            else
            {
                negativeSideMesh.vertices = negativeSideVertices.ToArray();
                negativeSideMesh.triangles = negativeSideTriangles.ToArray();
                negativeSideMesh.normals = negativeSideNormals.ToArray();
                negativeSideMesh.uv = negativeSideUvs.ToArray();
            }
        }

        /// <summary>
        /// ���ο� �޽ø� ����ϴ� �޼ҵ�
        /// </summary>
        private void ComputeNewMeshes()
        {
            int[] meshTriangles = mesh.triangles;
            Vector3[] meshVerts = mesh.vertices;
            Vector3[] meshNormals = mesh.normals;
            Vector2[] meshUvs = mesh.uv;

            // �ε����� 3���� �����Ƿ�, �Ź� 3�� �����ϵ��� ��.
            for (int i = 0; i < meshTriangles.Length; i += 3)
            {
                Vector3 vert1 = meshVerts[meshTriangles[i]];
                int vert1Index = Array.IndexOf(meshVerts, vert1);
                Vector2 uv1 = meshUvs[vert1Index];
                Vector3 normal1 = meshNormals[vert1Index];
                bool vert1Side = plane.GetSide(vert1);

                Vector3 vert2 = meshVerts[meshTriangles[i + 1]];
                int vert2Index = Array.IndexOf(meshVerts, vert2);
                Vector2 uv2 = meshUvs[vert2Index];
                Vector3 normal2 = meshNormals[vert2Index];
                bool vert2Side = plane.GetSide(vert2);

                Vector3 vert3 = meshVerts[meshTriangles[i + 2]];
                bool vert3Side = plane.GetSide(vert3);
                int vert3Index = Array.IndexOf(meshVerts, vert3);
                Vector3 normal3 = meshNormals[vert3Index];
                Vector2 uv3 = meshUvs[vert3Index];

                if (vert1Side == vert2Side && vert2Side == vert3Side)
                {
                    MeshSide side = (vert1Side) ? MeshSide.Positive : MeshSide.Negative;
                    AddTrianglesNormalAndUvs(side, vert1, normal1, uv1, vert2, normal2, uv2, vert3, normal3, uv3, true, false);
                }
                else
                {
                    Vector3 intersection1;
                    Vector3 intersection2;

                    Vector2 intersection1Uv;
                    Vector2 intersection2Uv;

                    MeshSide side1 = (vert1Side) ? MeshSide.Positive : MeshSide.Negative;
                    MeshSide side2 = (vert1Side) ? MeshSide.Negative : MeshSide.Positive;

                    if (vert1Side == vert2Side)
                    {
                        intersection1 = GetRayPlaneIntersectionPointAndUv(vert2, uv2, vert3, uv3, out intersection1Uv);
                        intersection2 = GetRayPlaneIntersectionPointAndUv(vert3, uv3, vert1, uv1, out intersection2Uv);

                        AddTrianglesNormalAndUvs(side1, vert1, null, uv1, vert2, null, uv2, intersection1, null, intersection1Uv, useSharedVertices, false);
                        AddTrianglesNormalAndUvs(side1, vert1, null, uv1, intersection1, null, intersection1Uv, intersection2, null, intersection2Uv, useSharedVertices, false);

                        AddTrianglesNormalAndUvs(side2, intersection1, null, intersection1Uv, vert3, null, uv3, intersection2, null, intersection2Uv, useSharedVertices, false);
                    }
                    else if (vert1Side == vert3Side)
                    {
                        intersection1 = GetRayPlaneIntersectionPointAndUv(vert1, uv1, vert2, uv2, out intersection1Uv);
                        intersection2 = GetRayPlaneIntersectionPointAndUv(vert2, uv2, vert3, uv3, out intersection2Uv);

                        AddTrianglesNormalAndUvs(side1, vert1, null, uv1, intersection1, null, intersection1Uv, vert3, null, uv3, useSharedVertices, false);
                        AddTrianglesNormalAndUvs(side1, intersection1, null, intersection1Uv, intersection2, null, intersection2Uv, vert3, null, uv3, useSharedVertices, false);

                        AddTrianglesNormalAndUvs(side2, intersection1, null, intersection1Uv, vert2, null, uv2, intersection2, null, intersection2Uv, useSharedVertices, false);
                    }
                    else
                    {
                        intersection1 = GetRayPlaneIntersectionPointAndUv(vert1, uv1, vert2, uv2, out intersection1Uv);
                        intersection2 = GetRayPlaneIntersectionPointAndUv(vert1, uv1, vert3, uv3, out intersection2Uv);

                        AddTrianglesNormalAndUvs(side1, vert1, null, uv1, intersection1, null, intersection1Uv, intersection2, null, intersection2Uv, useSharedVertices, false);

                        AddTrianglesNormalAndUvs(side2, intersection1, null, intersection1Uv, vert2, null, uv2, vert3, null, uv3, useSharedVertices, false);
                        AddTrianglesNormalAndUvs(side2, intersection1, null, intersection1Uv, vert3, null, uv3, intersection2, null, intersection2Uv, useSharedVertices, false);
                    }

                    pointsAlongPlane.Add(intersection1);
                    pointsAlongPlane.Add(intersection2);
                }
            }

            if (isSolid)
            {
                JoinPointsAlongPlane();
            }
            else if (createReverseTriangleWindings)
            {
                AddReverseTriangleWinding();
            }

            if (smoothVertices)
            {
                SmoothVertices();
            }
        }

        /// <summary>
        /// �� ������ �̾� ������ ���� ���� �� ���ο� UV�� ��� �޼ҵ�
        /// </summary>
        /// <param name="vertex1">ù ��° ����</param>
        /// <param name="vertex1Uv">ù ��° ������ UV</param>
        /// <param name="vertex2">�� ��° ����</param>
        /// <param name="vertex2Uv">�� ��° ������ UV</param>
        /// <param name="uv">���ο� UV</param>
        /// <returns>���� ����</returns>
        private Vector3 GetRayPlaneIntersectionPointAndUv(Vector3 vertex1, Vector2 vertex1Uv, Vector3 vertex2, Vector2 vertex2Uv, out Vector2 uv)
        {
            float distance = GetDistanceRelativeToPlane(vertex1, vertex2, out Vector3 pointOfIntersection);
            uv = InterpolateUvs(vertex1Uv, vertex2Uv, distance);
            return pointOfIntersection;
        }


        /*private Vector3 GetRayPlaneIntersectionPointAndUv(Vector3 vertex1, Vector2 vertex1Uv, Vector3 vertex2, Vector2 vertex2Uv, out Vector2 uv)
{
    // ���̸� �����մϴ�. �� ������ ���� ������� ���� ���ͷ� ������ ������ �����մϴ�.
    Ray ray = new Ray(vertex1, vertex2 - vertex1);

    // ���� ������ �������� ����մϴ�.
    if (plane.Raycast(ray, out float distance))
    {
        // ������ �������� ����մϴ�.
        Vector3 intersectionPoint = ray.GetPoint(distance);

        // �� ���� ������ �Ÿ��� ���� UV�� �����մϴ�.
        float totalDistance = Vector3.Distance(vertex1, vertex2);
        float distanceFromVertex1 = Vector3.Distance(vertex1, intersectionPoint);
        float interpolationFactor = distanceFromVertex1 / totalDistance;
        uv = Vector2.Lerp(vertex1Uv, vertex2Uv, interpolationFactor);

        return intersectionPoint;
    }
    else
    {
        // ���̿� ����� ������ ��� �Ǵ� ���̰� ���� �������� �ʴ� ���, ���� ó���� �մϴ�.
        Debug.LogWarning("Ray does not intersect with the plane.");
        uv = Vector2.zero;
        return Vector3.zero;
    }
}
*/

        /// <summary>
        /// ��鿡 ���� �Ÿ��� ����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="vertex1">ù ��° ����</param>
        /// <param name="vertex2">�� ��° ����</param>
        /// <param name="pointOfintersection">���� ����</param>
        /// <returns>�Ÿ�</returns>
        private float GetDistanceRelativeToPlane(Vector3 vertex1, Vector3 vertex2, out Vector3 pointOfintersection)
        {
            Ray ray = new Ray(vertex1, (vertex2 - vertex1));
            plane.Raycast(ray, out float distance);
            pointOfintersection = ray.GetPoint(distance);
            return distance;
        }

        /// <summary>
        /// �� UV ������ �Ÿ��� ���� ������ �����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="uv1">ù ��° UV</param>
        /// <param name="uv2">�� ��° UV</param>
        /// <param name="distance">�Ÿ�</param>
        /// <returns>������ UV</returns>
        private Vector2 InterpolateUvs(Vector2 uv1, Vector2 uv2, float distance)
        {
            Vector2 uv = Vector2.Lerp(uv1, uv2, distance);
            return uv;
        }

        /// <summary>
        /// �� ������ ���� ���� ���͸� ����ϴ� �޼ҵ�
        /// </summary>
        /// <param name="vertex1">ù ��° ����</param>
        /// <param name="vertex2">�� ��° ����</param>
        /// <param name="vertex3">�� ��° ����</param>
        /// <returns>���� ����</returns>
        private Vector3 ComputeNormal(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
        {
            Vector3 side1 = vertex2 - vertex1;
            Vector3 side2 = vertex3 - vertex1;

            Vector3 normal = Vector3.Cross(side1, side2);

            return normal;
        }

        /// <summary>
        /// �־��� ��� ���� ���� ���͸� ������ �޼ҵ�
        /// </summary>
        /// <param name="currentNormals">���� ���� ���</param>
        /// <returns>������ ���� ���� ���</returns>
        private List<Vector3> FlipNormals(List<Vector3> currentNormals)
        {
            List<Vector3> flippedNormals = new List<Vector3>();

            foreach (Vector3 normal in currentNormals)
            {
                flippedNormals.Add(-normal);
            }

            return flippedNormals;
        }

        /// <summary>
        /// ������ �ε巴�� ����� �޼ҵ�
        /// </summary>
        private void SmoothVertices()
        {
            DoSmoothing(ref positiveSideVertices, ref positiveSideNormals, ref positiveSideTriangles);
            DoSmoothing(ref negativeSideVertices, ref negativeSideNormals, ref negativeSideTriangles);
        }

        /// <summary>
        /// ������ �ε巴�� ����� ���� ����� �޼ҵ�
        /// </summary>
        /// <param name="vertices">���� ���</param>
        /// <param name="normals">���� ���� ���</param>
        /// <param name="triangles">�ﰢ�� ���</param>
        private void DoSmoothing(ref List<Vector3> vertices, ref List<Vector3> normals, ref List<int> triangles)
        {
            normals.ForEach(x =>
            {
                x = Vector3.zero;
            });

            for (int i = 0; i < triangles.Count; i += 3)
            {
                int vertIndex1 = triangles[i];
                int vertIndex2 = triangles[i + 1];
                int vertIndex3 = triangles[i + 2];

                Vector3 triangleNormal = ComputeNormal(vertices[vertIndex1], vertices[vertIndex2], vertices[vertIndex3]);

                normals[vertIndex1] += triangleNormal;
                normals[vertIndex2] += triangleNormal;
                normals[vertIndex3] += triangleNormal;
            }

            normals.ForEach(x =>
            {
                x.Normalize();
            });
        }
    }
}