using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    class Slicer
    {
        // 최대 허용 슬라이스 개수
        private const int MAX_SLICES_ALLOWED = 100;

        // 여러 객체를 절단하는 메서드
        public static List<GameObject> Slice(Plane plane, List<GameObject> objectsToCut)
        {
            List<GameObject> allSlices = new List<GameObject>();

            foreach (var objectToCut in objectsToCut)
            {
                // 단일 객체를 슬라이스
                var slices = SliceSingleObject(plane, objectToCut);
                if (slices != null)
                {
                    allSlices.AddRange(slices);
                    // 슬라이스 개수 제한 확인
                    if (allSlices.Count >= MAX_SLICES_ALLOWED)
                    {
                        Debug.LogWarning("Slice limit reached. Additional slicing operations will be skipped.");
                        break; // 제한 초과 시 루프 종료
                    }
                }
            }

            return allSlices;
        }

        // 단일 객체를 슬라이스
        private static GameObject[] SliceSingleObject(Plane plane, GameObject objectToCut)
        {
            Mesh mesh = objectToCut.GetComponent<MeshFilter>().mesh;
            Sliceable sliceable = objectToCut.GetComponent<Sliceable>();

            if (sliceable == null)
            {
                Debug.LogWarning($"Cannot slice: {objectToCut.name} does not have a Sliceable component.");
                return null;
            }

            // 슬라이스 메타데이터 생성
            SliceDummyData sliceData = new SliceDummyData(
                plane,
                mesh,
                sliceable.IsClosedMesh,
                sliceable.ReverseWindTriangle,
                sliceable.ShareVertex,
                sliceable.SmoothVertex
            );

            // 유효성 검사
            if (!IsSliceValid(sliceData.SideAMesh, 0.01f) || !IsSliceValid(sliceData.SideBMesh, 0.01f))
            {
                Debug.LogWarning("Generated slices are too small, slicing operation aborted.");
                return null;
            }

            // 슬라이스된 객체 생성
            GameObject SliceSideA = CreateSliceObject(objectToCut, $"{objectToCut.name}_A");
            GameObject SliceSideB = CreateSliceObject(objectToCut, $"{objectToCut.name}_B");

            SliceSideA.GetComponent<MeshFilter>().mesh = sliceData.SideAMesh;
            SliceSideB.GetComponent<MeshFilter>().mesh = sliceData.SideBMesh;

            SetupCollidersAndRigidBody(SliceSideA, sliceData.SideAMesh);
            SetupCollidersAndRigidBody(SliceSideB, sliceData.SideBMesh);

            // 원본 객체 삭제
            GameObject.Destroy(objectToCut);

            return new GameObject[] { SliceSideA, SliceSideB };
        }

        private static GameObject CreateSliceObject(GameObject originalObject, string newName)
        {
            GameObject sliceObject = new GameObject(newName);
            var originalMaterial = originalObject.GetComponent<MeshRenderer>().materials;
            Sliceable originalSliceable = originalObject.GetComponent<Sliceable>();

            sliceObject.AddComponent<MeshFilter>();
            sliceObject.AddComponent<MeshRenderer>().materials = originalMaterial;
            var sliceable = sliceObject.AddComponent<Sliceable>();

            sliceable.IsClosedMesh = originalSliceable.IsClosedMesh;
            sliceable.ReverseWindTriangle = originalSliceable.ReverseWindTriangle;

            sliceObject.transform.SetPositionAndRotation(
                originalObject.transform.position,
                originalObject.transform.rotation
            );
            sliceObject.transform.localScale = originalObject.transform.localScale;
            sliceObject.tag = originalObject.tag;

            return sliceObject;
        }

        private static bool IsSliceValid(Mesh mesh, float minArea)
        {
            return mesh.bounds.size.sqrMagnitude > minArea;
        }

        private static void SetupCollidersAndRigidBody(GameObject gameObject, Mesh mesh)
        {
            var meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            meshCollider.convex = true;

            var rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.useGravity = true;
        }
    }
}
