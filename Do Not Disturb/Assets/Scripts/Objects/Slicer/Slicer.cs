using System;
using UnityEngine;

namespace Assets.Scripts
{
    class Slicer
    {
        // 평면에 따라 객체를 자르기
        public static GameObject[] Slice(Plane plane, GameObject objectToCut)
        {
            Mesh mesh = objectToCut.GetComponent<MeshFilter>().mesh;
            Sliceable sliceable = objectToCut.GetComponent<Sliceable>();

            if (sliceable == null)
            {
                //Debug.LogWarning($"Cannot slice: {objectToCut.name} does not have a Sliceable component.");
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

            // 양측 생성
            GameObject SliceSideA = CreateSliceObject(objectToCut, $"{objectToCut.name}_A");
            GameObject SliceSideB = CreateSliceObject(objectToCut, $"{objectToCut.name}_B");

            SliceSideA.GetComponent<MeshFilter>().mesh = sliceData.SideAMesh;
            SliceSideB.GetComponent<MeshFilter>().mesh = sliceData.SideBMesh;

            SetupCollidersAndRigidBody(SliceSideA, sliceData.SideAMesh);
            SetupCollidersAndRigidBody(SliceSideB, sliceData.SideBMesh);

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
