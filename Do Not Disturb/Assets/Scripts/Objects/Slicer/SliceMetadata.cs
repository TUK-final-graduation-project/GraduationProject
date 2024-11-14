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
    public enum MeshSide
    {
        SideA = 0,
        SideB = 1
    }

    class SliceDummyData
    {
        private Mesh sideAMesh;
        private List<Vector3> sideAVertices;
        private List<int> sideATriangles;
        private List<Vector2> sideAUvs;
        private List<Vector3> sideANormals;

        private Mesh sideBMesh;
        private List<Vector3> sideBVertices;
        private List<int> sideBTriangles;
        private List<Vector2> sideBUvs;
        private List<Vector3> sideBNormals;

        private readonly List<Vector3> pointsAlongPlane;
        private Plane plane;
        private Mesh mesh;
        private bool isClosedMesh;
        private bool useSharedVertices = false;
        private bool smoothVertices = false;
        private bool createReverseTriangleWindings = false;

        // 양수 면 메시
        public Mesh SideAMesh
        {
            get
            {
                if (sideAMesh == null)
                {
                    sideAMesh = new Mesh();
                }

                SetMeshData(MeshSide.SideA);
                return sideAMesh;
            }
        }

        // 음수 면 메시
        public Mesh SideBMesh
        {
            get
            {
                if (sideBMesh == null)
                {
                    sideBMesh = new Mesh();
                }

                SetMeshData(MeshSide.SideB);

                return sideBMesh;
            }
        }

        public bool IsClosedMesh
        {
            get
            {
                return isClosedMesh;
            }
            set
            {
                isClosedMesh = value;
            }
        }

        // 슬라이스된 객체의 초기화 및 메시 계산을 담당하는 생성자
        public SliceDummyData(Plane plane, Mesh mesh, bool isSolid, bool createReverseTriangleWindings, bool shareVertices, bool smoothVertices)
        {
            sideATriangles = new List<int>();
            sideAVertices = new List<Vector3>();
            sideBTriangles = new List<int>();
            sideBVertices = new List<Vector3>();
            sideAUvs = new List<Vector2>();
            sideBUvs = new List<Vector2>();
            sideANormals = new List<Vector3>();
            sideBNormals = new List<Vector3>();
            pointsAlongPlane = new List<Vector3>();
            this.plane = plane;
            this.mesh = mesh;
            this.isClosedMesh = isSolid;
            this.createReverseTriangleWindings = createReverseTriangleWindings;
            useSharedVertices = shareVertices;
            this.smoothVertices = smoothVertices;

            ComputeNewMeshes();
        }

        // 메시 데이터를 올바른 면에 추가하고 법선 벡터를 계산하는 메소드
        private void AddTrianglesNormalAndUvs(MeshSide side, Vector3 vertex1, Vector3? normal1, Vector2 uv1, Vector3 vertex2, Vector3? normal2, Vector2 uv2, Vector3 vertex3, Vector3? normal3, Vector2 uv3, bool shareVertices, bool addFirst)
        {
            if (side == MeshSide.SideA)
            {
                AddTrianglesNormalsAndUvs(ref sideAVertices, ref sideATriangles, ref sideANormals, ref sideAUvs, vertex1, normal1, uv1, vertex2, normal2, uv2, vertex3, normal3, uv3, shareVertices, addFirst);
            }
            else
            {
                AddTrianglesNormalsAndUvs(ref sideBVertices, ref sideBTriangles, ref sideBNormals, ref sideBUvs, vertex1, normal1, uv1, vertex2, normal2, uv2, vertex3, normal3, uv3, shareVertices, addFirst);
            }
        }

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
            int positiveVertsStartIndex = sideAVertices.Count;
            // 원본 정점 복제
            sideAVertices.AddRange(sideAVertices);
            sideAUvs.AddRange(sideAUvs);
            sideANormals.AddRange(FlipNormals(sideANormals));

            int numPositiveTriangles = sideATriangles.Count;

            // 역 방향 삼각형 추가
            for (int i = 0; i < numPositiveTriangles; i += 3)
            {
                sideATriangles.Add(positiveVertsStartIndex + sideATriangles[i]);
                sideATriangles.Add(positiveVertsStartIndex + sideATriangles[i + 2]);
                sideATriangles.Add(positiveVertsStartIndex + sideATriangles[i + 1]);
            }

            int negativeVertextStartIndex = sideBVertices.Count;
            // 원본 정점 복제
            sideBVertices.AddRange(sideBVertices);
            sideBUvs.AddRange(sideBUvs);
            sideBNormals.AddRange(FlipNormals(sideBNormals));

            int numNegativeTriangles = sideBTriangles.Count;

            // 역 방향 삼각형 추가
            for (int i = 0; i < numNegativeTriangles; i += 3)
            {
                sideBTriangles.Add(negativeVertextStartIndex + sideBTriangles[i]);
                sideBTriangles.Add(negativeVertextStartIndex + sideBTriangles[i + 2]);
                sideBTriangles.Add(negativeVertextStartIndex + sideBTriangles[i + 1]);
            }
        }


        // 평면을 따라 점들을 중간 지점에 연결하는 메소드
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
                    AddTrianglesNormalAndUvs(MeshSide.SideA, halfway, -normal3, Vector2.zero, firstVertex, -normal3, Vector2.zero, secondVertex, -normal3, Vector2.zero, false, true);
                    AddTrianglesNormalAndUvs(MeshSide.SideB, halfway, normal3, Vector2.zero, secondVertex, normal3, Vector2.zero, firstVertex, normal3, Vector2.zero, false, true);
                }
                else
                {
                    AddTrianglesNormalAndUvs(MeshSide.SideA, halfway, normal3, Vector2.zero, secondVertex, normal3, Vector2.zero, firstVertex, normal3, Vector2.zero, false, true);
                    AddTrianglesNormalAndUvs(MeshSide.SideB, halfway, -normal3, Vector2.zero, firstVertex, -normal3, Vector2.zero, secondVertex, -normal3, Vector2.zero, false, true);
                }
            }
        }

        // 첫 번째 점과 가장 멀리 떨어진 점 사이의 중간 지점을 구하는 메소드
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

        // 메시 데이터를 설정하는 메소드
        private void SetMeshData(MeshSide side)
        {
            if (side == MeshSide.SideA)
            {
                sideAMesh.vertices = sideAVertices.ToArray();
                sideAMesh.triangles = sideATriangles.ToArray();
                sideAMesh.normals = sideANormals.ToArray();
                sideAMesh.uv = sideAUvs.ToArray();
            }
            else
            {
                sideBMesh.vertices = sideBVertices.ToArray();
                sideBMesh.triangles = sideBTriangles.ToArray();
                sideBMesh.normals = sideBNormals.ToArray();
                sideBMesh.uv = sideBUvs.ToArray();
            }
        }

        // 새로운 메시를 계산하는 메소드
        private void ComputeNewMeshes()
        {
            int[] meshTriangles = mesh.triangles;
            Vector3[] meshVertices = mesh.vertices;
            Vector3[] meshNormals = mesh.normals;
            Vector2[] meshUvs = mesh.uv;

            // 인덱스를 3개씩 읽으므로, 매번 3씩 증가하도록 함.
            for (int i = 0; i < meshTriangles.Length; i += 3)
            {
                Vector3 v1 = meshVertices[meshTriangles[i]];
                int v1Index = Array.IndexOf(meshVertices, v1);
                Vector2 uv1 = meshUvs[v1Index];
                Vector3 normal1 = meshNormals[v1Index];
                bool v1Side = plane.GetSide(v1);

                Vector3 v2 = meshVertices[meshTriangles[i + 1]];
                int v2Index = Array.IndexOf(meshVertices, v2);
                Vector2 uv2 = meshUvs[v2Index];
                Vector3 normal2 = meshNormals[v2Index];
                bool v2Side = plane.GetSide(v2);

                Vector3 v3 = meshVertices[meshTriangles[i + 2]];
                bool v3Side = plane.GetSide(v3);
                int v3Index = Array.IndexOf(meshVertices, v3);
                Vector3 normal3 = meshNormals[v3Index];
                Vector2 uv3 = meshUvs[v3Index];

                if (v1Side == v2Side && v2Side == v3Side)
                {
                    MeshSide side = (v1Side) ? MeshSide.SideA : MeshSide.SideB;
                    AddTrianglesNormalAndUvs(side, v1, normal1, uv1, v2, normal2, uv2, v3, normal3, uv3, true, false);
                }
                else
                {
                    Vector3 intersection1;
                    Vector3 intersection2;

                    Vector2 intersection1Uv;
                    Vector2 intersection2Uv;

                    MeshSide side1 = (v1Side) ? MeshSide.SideA : MeshSide.SideB;
                    MeshSide side2 = (v1Side) ? MeshSide.SideB : MeshSide.SideA;

                    if (v1Side == v2Side)
                    {
                        intersection1 = GetRayPlaneIntersectionPointAndUv(v2, uv2, v3, uv3, out intersection1Uv);
                        intersection2 = GetRayPlaneIntersectionPointAndUv(v3, uv3, v1, uv1, out intersection2Uv);

                        AddTrianglesNormalAndUvs(side1, v1, null, uv1, v2, null, uv2, intersection1, null, intersection1Uv, useSharedVertices, false);
                        AddTrianglesNormalAndUvs(side1, v1, null, uv1, intersection1, null, intersection1Uv, intersection2, null, intersection2Uv, useSharedVertices, false);

                        AddTrianglesNormalAndUvs(side2, intersection1, null, intersection1Uv, v3, null, uv3, intersection2, null, intersection2Uv, useSharedVertices, false);
                    }
                    else if (v1Side == v3Side)
                    {
                        intersection1 = GetRayPlaneIntersectionPointAndUv(v1, uv1, v2, uv2, out intersection1Uv);
                        intersection2 = GetRayPlaneIntersectionPointAndUv(v2, uv2, v3, uv3, out intersection2Uv);

                        AddTrianglesNormalAndUvs(side1, v1, null, uv1, intersection1, null, intersection1Uv, v3, null, uv3, useSharedVertices, false);
                        AddTrianglesNormalAndUvs(side1, intersection1, null, intersection1Uv, intersection2, null, intersection2Uv, v3, null, uv3, useSharedVertices, false);

                        AddTrianglesNormalAndUvs(side2, intersection1, null, intersection1Uv, v2, null, uv2, intersection2, null, intersection2Uv, useSharedVertices, false);
                    }
                    else
                    {
                        intersection1 = GetRayPlaneIntersectionPointAndUv(v1, uv1, v2, uv2, out intersection1Uv);
                        intersection2 = GetRayPlaneIntersectionPointAndUv(v1, uv1, v3, uv3, out intersection2Uv);

                        AddTrianglesNormalAndUvs(side1, v1, null, uv1, intersection1, null, intersection1Uv, intersection2, null, intersection2Uv, useSharedVertices, false);

                        AddTrianglesNormalAndUvs(side2, intersection1, null, intersection1Uv, v2, null, uv2, v3, null, uv3, useSharedVertices, false);
                        AddTrianglesNormalAndUvs(side2, intersection1, null, intersection1Uv, v3, null, uv3, intersection2, null, intersection2Uv, useSharedVertices, false);
                    }

                    pointsAlongPlane.Add(intersection1);
                    pointsAlongPlane.Add(intersection2);
                }
            }

            if (isClosedMesh)
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

        // 두 정점을 이어 평면과의 교차 지점 및 새로운 UV를 얻는 메소드
        private Vector3 GetRayPlaneIntersectionPointAndUv(Vector3 v1, Vector2 v1Uv, Vector3 v2, Vector2 v2Uv, out Vector2 uv)
        {
            float distance = GetDistanceRelativeToPlane(v1, v2, out Vector3 pointOfIntersection);
            uv = InterpolateUvs(v1Uv, v2Uv, distance);
            return pointOfIntersection;
        }

        // 평면에 대한 거리를 계산하는 메소드
        private float GetDistanceRelativeToPlane(Vector3 v1, Vector3 v2, out Vector3 pointOfintersection)
        {
            Ray ray = new Ray(v1, (v2 - v1));
            plane.Raycast(ray, out float distance);
            pointOfintersection = ray.GetPoint(distance);
            return distance;
        }
        
        // 두 UV 사이의 거리에 대한 보간을 수행하는 메소드
        private Vector2 InterpolateUvs(Vector2 uv1, Vector2 uv2, float distance)
        {
            Vector2 uv = Vector2.Lerp(uv1, uv2, distance);
            return uv;
        }
      
        // 세 정점에 대한 법선 벡터를 계산하는 메소드
        private Vector3 ComputeNormal(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            Vector3 side1 = v2 - v1;
            Vector3 side2 = v3 - v1;

            Vector3 normal = Vector3.Cross(side1, side2);

            return normal;
        }
        
        // 주어진 목록 내의 법선 벡터를 뒤집는 메소드
        private List<Vector3> FlipNormals(List<Vector3> currentNormals)
        {
            List<Vector3> flippedNormals = new List<Vector3>();

            foreach (Vector3 normal in currentNormals)
            {
                flippedNormals.Add(-normal);
            }

            return flippedNormals;
        }

        // 정점을 부드럽게 만드는 메소드
        private void SmoothVertices()
        {
            DoSmoothing(ref sideAVertices, ref sideANormals, ref sideATriangles);
            DoSmoothing(ref sideBVertices, ref sideBNormals, ref sideBTriangles);
        }

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