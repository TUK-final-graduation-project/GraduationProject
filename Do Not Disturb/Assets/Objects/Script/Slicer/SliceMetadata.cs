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
    /// 메시의 측면을 나타내는 열거형
    /// </summary>
    public enum MeshSide
    {
        Positive = 0,
        Negative = 1
    }

    /// <summary>
    /// 슬라이스된 객체의 양쪽 면 메시 데이터를 관리하는 객체
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


        // 객체가 실체인지 여부를 나타내는 프로퍼티
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

        // 양수 면 메시를 가져오는 프로퍼티
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

        // 음수 면 메시를 가져오는 프로퍼티
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


        // 슬라이스된 객체의 초기화 및 메시 계산을 담당하는 생성자

        /// <param name="plane">슬라이스 평면</param>
        /// <param name="mesh">원본 메시</param>
        /// <param name="isSolid">실체인지 여부</param>
        /// <param name="createReverseTriangleWindings">역 삼각형 트라이앵글을 만들지 여부</param>
        /// <param name="shareVertices">공유된 정점 사용 여부</param>
        /// <param name="smoothVertices">정점을 부드럽게 처리할지 여부</param>

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

        // 메시 데이터를 올바른 면에 추가하고 법선 벡터를 계산하는 메소드

        /// <param name="side">추가할 면</param>
        /// <param name="vertex1">첫 번째 정점</param>
        /// <param name="normal1">첫 번째 정점 법선 벡터</param>
        /// <param name="uv1">첫 번째 정점 UV 좌표</param>
        /// <param name="vertex2">두 번째 정점</param>
        /// <param name="normal2">두 번째 정점 법선 벡터</param>
        /// <param name="uv2">두 번째 정점 UV 좌표</param>
        /// <param name="vertex3">세 번째 정점</param>
        /// <param name="normal3">세 번째 정점 법선 벡터</param>
        /// <param name="uv3">세 번째 정점 UV 좌표</param>
        /// <param name="shareVertices">정점을 공유할지 여부</param>
        /// <param name="addFirst">첫 번째 정점을 먼저 추가할지 여부</param>
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

        // 정점을 메시에 추가하고 정점 순서에 따라 삼각형을 설정합니다.
        // 공유된 정점이 아닌 경우 일치하는 정점이 이미 존재하더라도 정점이 추가됩니다.
        // 법선 벡터를 계산하지 않습니다.

        /// <param name="vertices">정점 리스트</param>
        /// <param name="triangles">삼각형 리스트</param>
        /// <param name="uvs">UV 좌표 리스트</param>
        /// <param name="normals">법선 벡터 리스트</param>
        /// <param name="vertex1">첫 번째 정점</param>
        /// <param name="normal1">첫 번째 정점 법선 벡터</param>
        /// <param name="uv1">첫 번째 정점 UV 좌표</param>
        /// <param name="vertex2">두 번째 정점</param>
        /// <param name="normal2">두 번째 정점 법선 벡터</param>
        /// <param name="uv2">두 번째 정점 UV 좌표</param>
        /// <param name="vertex3">세 번째 정점</param>
        /// <param name="normal3">세 번째 정점 법선 벡터</param>
        /// <param name="uv3">세 번째 정점 UV 좌표</param>
        /// <param name="shareVertices">정점을 공유할지 여부</param>
        /// <param name="addFirst">첫 번째 정점을 먼저 추가할지 여부</param>
        private void AddTrianglesNormalsAndUvs(ref List<Vector3> vertices, ref List<int> triangles, ref List<Vector3> normals, ref List<Vector2> uvs, Vector3 vertex1, Vector3? normal1, Vector2 uv1, Vector3 vertex2, Vector3? normal2, Vector2 uv2, Vector3 vertex3, Vector3? normal3, Vector2 uv3, bool shareVertices, bool addFirst)
        {
            int tri1Index = vertices.IndexOf(vertex1);

            if (addFirst)
            {
                ShiftTriangleIndeces(ref triangles);
            }

            // 정점이 이미 존재하는 경우, 정점에 대한 삼각형 참조를 추가하고 그렇지 않으면 정점을 리스트에 추가하고 삼각형 인덱스를 추가합니다.
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


        // 정점, 법선 벡터, UV 좌표 및 삼각형을 추가하는 메소드

        /// <param name="vertices">정점 리스트</param>
        /// <param name="normals">법선 벡터 리스트</param>
        /// <param name="uvs">UV 좌표 리스트</param>
        /// <param name="triangles">삼각형 리스트</param>
        /// <param name="vertex">정점</param>
        /// <param name="normal">법선 벡터</param>
        /// <param name="uv">UV 좌표</param>
        /// <param name="index">삽입할 인덱스</param>
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

        // 삼각형 인덱스를 조정하여 메시를 변환하는 메소드

        /// <param name="triangles">삼각형 리스트</param>
        private void ShiftTriangleIndeces(ref List<int> triangles)
        {
            for (int j = 0; j < triangles.Count; j += 3)
            {
                triangles[j] += +3;
                triangles[j + 1] += 3;
                triangles[j + 2] += 3;
            }
        }

        // 객체의 내부를 렌더링하는 메소드
        // 모든 정점을 복제하고 역 삼각형 방향을 생성하여 비용이 많이 들 수 있다.
        private void AddReverseTriangleWinding()
        {
            int positiveVertsStartIndex = positiveSideVertices.Count;
            // 원본 정점 복제
            positiveSideVertices.AddRange(positiveSideVertices);
            positiveSideUvs.AddRange(positiveSideUvs);
            positiveSideNormals.AddRange(FlipNormals(positiveSideNormals));

            int numPositiveTriangles = positiveSideTriangles.Count;

            // 역 방향 삼각형 추가
            for (int i = 0; i < numPositiveTriangles; i += 3)
            {
                positiveSideTriangles.Add(positiveVertsStartIndex + positiveSideTriangles[i]);
                positiveSideTriangles.Add(positiveVertsStartIndex + positiveSideTriangles[i + 2]);
                positiveSideTriangles.Add(positiveVertsStartIndex + positiveSideTriangles[i + 1]);
            }

            int negativeVertextStartIndex = negativeSideVertices.Count;
            // 원본 정점 복제
            negativeSideVertices.AddRange(negativeSideVertices);
            negativeSideUvs.AddRange(negativeSideUvs);
            negativeSideNormals.AddRange(FlipNormals(negativeSideNormals));

            int numNegativeTriangles = negativeSideTriangles.Count;

            // 역 방향 삼각형 추가
            for (int i = 0; i < numNegativeTriangles; i += 3)
            {
                negativeSideTriangles.Add(negativeVertextStartIndex + negativeSideTriangles[i]);
                negativeSideTriangles.Add(negativeVertextStartIndex + negativeSideTriangles[i + 2]);
                negativeSideTriangles.Add(negativeVertextStartIndex + negativeSideTriangles[i + 1]);
            }
        }


        /// <summary>
        /// 평면을 따라 점들을 중간 지점에 연결하는 메소드
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
        /// 첫 번째 점과 가장 멀리 떨어진 점 사이의 중간 지점을 구하는 메소드
        /// </summary>
        /// <param name="distance">가장 먼 점과의 거리</param>
        /// <returns>중간 지점</returns>
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
        /// 메시 데이터를 설정하는 메소드
        /// </summary>
        /// <param name="side">메시의 면</param>
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
        /// 새로운 메시를 계산하는 메소드
        /// </summary>
        private void ComputeNewMeshes()
        {
            int[] meshTriangles = mesh.triangles;
            Vector3[] meshVerts = mesh.vertices;
            Vector3[] meshNormals = mesh.normals;
            Vector2[] meshUvs = mesh.uv;

            // 인덱스를 3개씩 읽으므로, 매번 3씩 증가하도록 함.
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
        /// 두 정점을 이어 평면과의 교차 지점 및 새로운 UV를 얻는 메소드
        /// </summary>
        /// <param name="vertex1">첫 번째 정점</param>
        /// <param name="vertex1Uv">첫 번째 정점의 UV</param>
        /// <param name="vertex2">두 번째 정점</param>
        /// <param name="vertex2Uv">두 번째 정점의 UV</param>
        /// <param name="uv">새로운 UV</param>
        /// <returns>교차 지점</returns>
        private Vector3 GetRayPlaneIntersectionPointAndUv(Vector3 vertex1, Vector2 vertex1Uv, Vector3 vertex2, Vector2 vertex2Uv, out Vector2 uv)
        {
            float distance = GetDistanceRelativeToPlane(vertex1, vertex2, out Vector3 pointOfIntersection);
            uv = InterpolateUvs(vertex1Uv, vertex2Uv, distance);
            return pointOfIntersection;
        }


        /*private Vector3 GetRayPlaneIntersectionPointAndUv(Vector3 vertex1, Vector2 vertex1Uv, Vector3 vertex2, Vector2 vertex2Uv, out Vector2 uv)
{
    // 레이를 정의합니다. 두 정점을 통해 만들어진 방향 벡터로 레이의 방향을 결정합니다.
    Ray ray = new Ray(vertex1, vertex2 - vertex1);

    // 평면과 레이의 교차점을 계산합니다.
    if (plane.Raycast(ray, out float distance))
    {
        // 레이의 교차점을 계산합니다.
        Vector3 intersectionPoint = ray.GetPoint(distance);

        // 두 정점 사이의 거리에 따라 UV를 보간합니다.
        float totalDistance = Vector3.Distance(vertex1, vertex2);
        float distanceFromVertex1 = Vector3.Distance(vertex1, intersectionPoint);
        float interpolationFactor = distanceFromVertex1 / totalDistance;
        uv = Vector2.Lerp(vertex1Uv, vertex2Uv, interpolationFactor);

        return intersectionPoint;
    }
    else
    {
        // 레이와 평면이 평행한 경우 또는 레이가 평면과 교차하지 않는 경우, 예외 처리를 합니다.
        Debug.LogWarning("Ray does not intersect with the plane.");
        uv = Vector2.zero;
        return Vector3.zero;
    }
}
*/

        /// <summary>
        /// 평면에 대한 거리를 계산하는 메소드
        /// </summary>
        /// <param name="vertex1">첫 번째 정점</param>
        /// <param name="vertex2">두 번째 정점</param>
        /// <param name="pointOfintersection">교차 지점</param>
        /// <returns>거리</returns>
        private float GetDistanceRelativeToPlane(Vector3 vertex1, Vector3 vertex2, out Vector3 pointOfintersection)
        {
            Ray ray = new Ray(vertex1, (vertex2 - vertex1));
            plane.Raycast(ray, out float distance);
            pointOfintersection = ray.GetPoint(distance);
            return distance;
        }

        /// <summary>
        /// 두 UV 사이의 거리에 대한 보간을 수행하는 메소드
        /// </summary>
        /// <param name="uv1">첫 번째 UV</param>
        /// <param name="uv2">두 번째 UV</param>
        /// <param name="distance">거리</param>
        /// <returns>보간된 UV</returns>
        private Vector2 InterpolateUvs(Vector2 uv1, Vector2 uv2, float distance)
        {
            Vector2 uv = Vector2.Lerp(uv1, uv2, distance);
            return uv;
        }

        /// <summary>
        /// 세 정점에 대한 법선 벡터를 계산하는 메소드
        /// </summary>
        /// <param name="vertex1">첫 번째 정점</param>
        /// <param name="vertex2">두 번째 정점</param>
        /// <param name="vertex3">세 번째 정점</param>
        /// <returns>법선 벡터</returns>
        private Vector3 ComputeNormal(Vector3 vertex1, Vector3 vertex2, Vector3 vertex3)
        {
            Vector3 side1 = vertex2 - vertex1;
            Vector3 side2 = vertex3 - vertex1;

            Vector3 normal = Vector3.Cross(side1, side2);

            return normal;
        }

        /// <summary>
        /// 주어진 목록 내의 법선 벡터를 뒤집는 메소드
        /// </summary>
        /// <param name="currentNormals">법선 벡터 목록</param>
        /// <returns>뒤집힌 법선 벡터 목록</returns>
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
        /// 정점을 부드럽게 만드는 메소드
        /// </summary>
        private void SmoothVertices()
        {
            DoSmoothing(ref positiveSideVertices, ref positiveSideNormals, ref positiveSideTriangles);
            DoSmoothing(ref negativeSideVertices, ref negativeSideNormals, ref negativeSideTriangles);
        }

        /// <summary>
        /// 정점을 부드럽게 만드는 내부 도우미 메소드
        /// </summary>
        /// <param name="vertices">정점 목록</param>
        /// <param name="normals">법선 벡터 목록</param>
        /// <param name="triangles">삼각형 목록</param>
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