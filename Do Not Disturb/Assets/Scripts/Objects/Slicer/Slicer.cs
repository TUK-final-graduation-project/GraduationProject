using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    class Slicer
    {
        // �ִ� ��� �����̽� ����
        private const int MAX_SLICES_ALLOWED = 100;

        // ���� ��ü�� �����ϴ� �޼���
        public static List<GameObject> Slice(Plane plane, List<GameObject> objectsToCut)
        {
            List<GameObject> allSlices = new List<GameObject>();

            foreach (var objectToCut in objectsToCut)
            {
                // ���� ��ü�� �����̽�
                var slices = SliceSingleObject(plane, objectToCut);
                if (slices != null)
                {
                    allSlices.AddRange(slices);
                    // �����̽� ���� ���� Ȯ��
                    if (allSlices.Count >= MAX_SLICES_ALLOWED)
                    {
                        Debug.LogWarning("Slice limit reached. Additional slicing operations will be skipped.");
                        break; // ���� �ʰ� �� ���� ����
                    }
                }
            }

            return allSlices;
        }

        // ���� ��ü�� �����̽�
        private static GameObject[] SliceSingleObject(Plane plane, GameObject objectToCut)
        {
            Mesh mesh = objectToCut.GetComponent<MeshFilter>().mesh;
            Sliceable sliceable = objectToCut.GetComponent<Sliceable>();

            if (sliceable == null)
            {
                Debug.LogWarning($"Cannot slice: {objectToCut.name} does not have a Sliceable component.");
                return null;
            }

            // �����̽� ��Ÿ������ ����
            SliceDummyData sliceData = new SliceDummyData(
                plane,
                mesh,
                sliceable.IsClosedMesh,
                sliceable.ReverseWindTriangle,
                sliceable.ShareVertex,
                sliceable.SmoothVertex
            );

            // ��ȿ�� �˻�
            if (!IsSliceValid(sliceData.SideAMesh, 0.01f) || !IsSliceValid(sliceData.SideBMesh, 0.01f))
            {
                Debug.LogWarning("Generated slices are too small, slicing operation aborted.");
                return null;
            }

            // �����̽��� ��ü ����
            GameObject SliceSideA = CreateSliceObject(objectToCut, $"{objectToCut.name}_A");
            GameObject SliceSideB = CreateSliceObject(objectToCut, $"{objectToCut.name}_B");

            SliceSideA.GetComponent<MeshFilter>().mesh = sliceData.SideAMesh;
            SliceSideB.GetComponent<MeshFilter>().mesh = sliceData.SideBMesh;

            SetupCollidersAndRigidBody(SliceSideA, sliceData.SideAMesh);
            SetupCollidersAndRigidBody(SliceSideB, sliceData.SideBMesh);

            // ���� ��ü ����
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
