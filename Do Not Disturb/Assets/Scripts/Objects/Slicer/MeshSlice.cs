using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSlice : MonoBehaviour
{
    public static GameObject[] SlicerWorld(GameObject target, Vector3 sliceNormal, Vector3 slicePoint, Material interiorMaterial)
    {
        Vector3 localNormal = target.transform.InverseTransformVector(sliceNormal);
        Vector3 localPoint = target.transform.InverseTransformPoint(slicePoint);
        return Slicer(target, localNormal, localPoint, interiorMaterial);
    }

    public static GameObject[] Slicer(GameObject target, Vector3 sliceNormal, Vector3 slicePoint, Material interiorMaterial)
    {
        Mesh originalMesh = target.GetComponent<MeshFilter>().sharedMesh;
        Vector3[] originalVertices = originalMesh.vertices;
        Vector3[] originalNormals = originalMesh.normals;
        Vector2[] originalUVs = originalMesh.uv;
        int originalSubMeshCount = originalMesh.subMeshCount;
        Material[] originalMaterials = target.GetComponent<MeshRenderer>().sharedMaterials;

        int existingInteriorMatIndex = -1;
        for (int i = 0; i < originalMaterials.Length; i++)
        {
            if (originalMaterials[i].Equals(interiorMaterial))
            {
                existingInteriorMatIndex = i;
                break;
            }
        }

        List<Vector3> aSideVertices = new List<Vector3>();
        List<Vector3> bSideVertices = new List<Vector3>();
        List<Vector3> aSideNormals = new List<Vector3>();
        List<Vector3> bSideNormals = new List<Vector3>();
        List<Vector2> aSideUVs = new List<Vector2>();
        List<Vector2> bSideUVs = new List<Vector2>();
        List<int>[] aSideTriangles = new List<int>[originalSubMeshCount];
        List<int>[] bSideTriangles = new List<int>[originalSubMeshCount];

        List<Vector3> createdVertices = new List<Vector3>();
        List<Vector3> createdNormals = new List<Vector3>();
        List<Vector2> createdUVs = new List<Vector2>();

        for (int i = 0; i < originalSubMeshCount; i++)
        {
            int aVertCount = aSideVertices.Count;
            int bVertCount = bSideVertices.Count;

            ParseSubMesh(originalVertices, originalNormals, originalUVs, originalMesh.GetTriangles(i),
                         sliceNormal, slicePoint, ref aSideVertices, ref bSideVertices,
                         ref aSideNormals, ref bSideNormals, ref aSideUVs, ref bSideUVs,
                         out aSideTriangles[i], out bSideTriangles[i], ref createdVertices, ref createdNormals, ref createdUVs);

            for (int j = 0; j < aSideTriangles[i].Count; j++)
            {
                aSideTriangles[i][j] += aVertCount;
            }

            for (int j = 0; j < bSideTriangles[i].Count; j++)
            {
                bSideTriangles[i][j] += bVertCount;
            }
        }

        List<Vector3> sortedCreatedVertices;
        SortVertices(createdVertices, out sortedCreatedVertices);

        List<Vector3> aSideCapVertices, bSideCapVertices;
        List<Vector3> aSideCapNormals, bSideCapNormals;
        List<Vector2> aSideCapUVs, bSideCapUVs;
        List<int> aSideCapTriangles, bSideCapTriangles;

        MakeCap(sliceNormal, sortedCreatedVertices, out aSideCapVertices, out bSideCapVertices,
                out aSideCapNormals, out bSideCapNormals, out aSideCapUVs, out bSideCapUVs,
                out aSideCapTriangles, out bSideCapTriangles);

        for (int i = 0; i < aSideCapTriangles.Count; i++)
        {
            aSideCapTriangles[i] += aSideVertices.Count;
        }
        for (int i = 0; i < bSideCapTriangles.Count; i++)
        {
            bSideCapTriangles[i] += bSideVertices.Count;
        }

        List<Vector3> aSideFinalVertices = new List<Vector3>();
        List<Vector3> bSideFinalVertices = new List<Vector3>();
        List<Vector3> aSideFinalNormals = new List<Vector3>();
        List<Vector3> bSideFinalNormals = new List<Vector3>();
        List<Vector2> aSideFinalUVs = new List<Vector2>();
        List<Vector2> bSideFinalUVs = new List<Vector2>();

        aSideFinalVertices.AddRange(aSideVertices);
        aSideFinalVertices.AddRange(aSideCapVertices);
        bSideFinalVertices.AddRange(bSideVertices);
        bSideFinalVertices.AddRange(bSideCapVertices);
        aSideFinalNormals.AddRange(aSideNormals);
        aSideFinalNormals.AddRange(aSideCapNormals);
        bSideFinalNormals.AddRange(bSideNormals);
        bSideFinalNormals.AddRange(bSideCapNormals);
        aSideFinalUVs.AddRange(aSideUVs);
        aSideFinalUVs.AddRange(aSideCapUVs);
        bSideFinalUVs.AddRange(bSideUVs);
        bSideFinalUVs.AddRange(bSideCapUVs);

        if (existingInteriorMatIndex > 0)
        {
            aSideTriangles[existingInteriorMatIndex].AddRange(aSideCapTriangles);
            bSideTriangles[existingInteriorMatIndex].AddRange(bSideCapTriangles);
        }

        Mesh aMesh = new Mesh();
        Mesh bMesh = new Mesh();
        aMesh.vertices = aSideFinalVertices.ToArray();
        aMesh.normals = aSideFinalNormals.ToArray();
        aMesh.uv = aSideFinalUVs.ToArray();
        aMesh.subMeshCount = existingInteriorMatIndex < 0 ? originalSubMeshCount + 1 : originalSubMeshCount;

        for (int i = 0; i < originalSubMeshCount; i++)
        {
            aMesh.SetTriangles(aSideTriangles[i], i);
        }

        if (existingInteriorMatIndex < 0) aMesh.SetTriangles(aSideCapTriangles, originalSubMeshCount);

        bMesh.vertices = bSideFinalVertices.ToArray();
        bMesh.normals = bSideFinalNormals.ToArray();
        bMesh.uv = bSideFinalUVs.ToArray();
        bMesh.subMeshCount = existingInteriorMatIndex < 0 ? originalSubMeshCount + 1 : originalSubMeshCount;

        for (int i = 0; i < originalSubMeshCount; i++)
        {
            bMesh.SetTriangles(bSideTriangles[i], i);
        }

        if (existingInteriorMatIndex < 0) bMesh.SetTriangles(bSideCapTriangles, originalSubMeshCount);

        GameObject aObject = new GameObject(target.name + "_A", typeof(MeshFilter), typeof(MeshRenderer));
        GameObject bObject = new GameObject(target.name + "_B", typeof(MeshFilter), typeof(MeshRenderer));
        Material[] mats = new Material[(existingInteriorMatIndex < 0 ? originalSubMeshCount + 1 : originalSubMeshCount)];

        for (int i = 0; i < originalSubMeshCount; i++)
        {
            mats[i] = originalMaterials[i];
        }

        if (existingInteriorMatIndex < 0) mats[originalSubMeshCount] = interiorMaterial;
        aObject.GetComponent<MeshFilter>().sharedMesh = aMesh;
        aObject.GetComponent<MeshRenderer>().sharedMaterials = mats;
        bObject.GetComponent<MeshFilter>().sharedMesh = bMesh;
        bObject.GetComponent<MeshRenderer>().sharedMaterials = mats;
        aObject.transform.position = target.transform.position;
        aObject.transform.rotation = target.transform.rotation;
        aObject.transform.localScale = target.transform.localScale;
        bObject.transform.position = target.transform.position;
        bObject.transform.rotation = target.transform.rotation;
        bObject.transform.localScale = target.transform.localScale;

        target.SetActive(false);

        return new GameObject[] { aObject, bObject };
    }

    internal static void SwapTwoIndex<T>(ref List<T> target, int idx0, int idx1)
    {
        T temp = target[idx1];
        target[idx1] = target[idx0];
        target[idx0] = temp;
    }

    internal static void SwapTwoIndexSet<T>(ref List<T> target, int idx00, int idx01, int idx10, int idx11)
    {
        T temp0 = target[idx00];
        T temp1 = target[idx01];
        target[idx00] = target[idx10];
        target[idx01] = target[idx11];
        target[idx10] = temp0;
        target[idx11] = temp1;
    }

    internal static void SortVertices(List<Vector3> target, out List<Vector3> result)
    {
        result = new List<Vector3>();
        result.Add(target[0]);
        result.Add(target[1]);

        int vertSetCount = target.Count / 2;

        for (int i = 0; i < vertSetCount - 1; i++)
        {
            Vector3 vert0 = target[i * 2];
            Vector3 vert1 = target[i * 2 + 1];

            for (int j = i + 1; j < vertSetCount; j++)
            {
                Vector3 cVert0 = target[j * 2];
                Vector3 cVert1 = target[j * 2 + 1];

                if (vert1 == cVert0)
                {
                    result.Add(cVert1);
                    SwapTwoIndexSet<Vector3>(ref target, i * 2 + 2, i * 2 + 3, j * 2, j * 2 + 1);
                }
                else if (vert1 == cVert1)
                {
                    result.Add(cVert0);
                    SwapTwoIndex<Vector3>(ref target, j * 2, j * 2 + 1);
                    SwapTwoIndexSet<Vector3>(ref target, i * 2 + 2, i * 2 + 3, j * 2, j * 2 + 1);
                }
            }
        }
    }

    internal static void ParseSubMesh(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, int[] indices,
        Vector3 sliceNormal, Vector3 slicePoint, ref List<Vector3> aSideVertices, ref List<Vector3> bSideVertices,
        ref List<Vector3> aSideNormals, ref List<Vector3> bSideNormals, ref List<Vector2> aSideUVs, ref List<Vector2> bSideUVs,
        out List<int> aSideIndices, out List<int> bSideIndices, ref List<Vector3> createdVertices, ref List<Vector3> createdNormals, ref List<Vector2> createdUVs)
    {
        aSideIndices = new List<int>();
        bSideIndices = new List<int>();

        for (int i = 0; i < indices.Length; i += 3)
        {
            List<Vector3> triVerts = new List<Vector3>() { vertices[indices[i]], vertices[indices[i + 1]], vertices[indices[i + 2]] };
            List<Vector3> triNorms = new List<Vector3>() { normals[indices[i]], normals[indices[i + 1]], normals[indices[i + 2]] };
            List<Vector2> triUVs = new List<Vector2>() { uvs[indices[i]], uvs[indices[i + 1]], uvs[indices[i + 2]] };
            float[] triSides = new float[] {
                Vector3.Dot(triVerts[0] - slicePoint, sliceNormal),
                Vector3.Dot(triVerts[1] - slicePoint, sliceNormal),
                Vector3.Dot(triVerts[2] - slicePoint, sliceNormal)
            };

            if (triSides[0] >= 0 && triSides[1] >= 0 && triSides[2] >= 0)
            {
                aSideVertices.AddRange(triVerts);
                aSideNormals.AddRange(triNorms);
                aSideUVs.AddRange(triUVs);
                aSideIndices.Add(aSideVertices.Count - 3);
                aSideIndices.Add(aSideVertices.Count - 2);
                aSideIndices.Add(aSideVertices.Count - 1);
            }
            else if (triSides[0] <= 0 && triSides[1] <= 0 && triSides[2] <= 0)
            {
                bSideVertices.AddRange(triVerts);
                bSideNormals.AddRange(triNorms);
                bSideUVs.AddRange(triUVs);
                bSideIndices.Add(bSideVertices.Count - 3);
                bSideIndices.Add(bSideVertices.Count - 2);
                bSideIndices.Add(bSideVertices.Count - 1);
            }
            else
            {
                ClipTriangle(triVerts, triNorms, triUVs, triSides, sliceNormal, slicePoint, ref aSideVertices, ref bSideVertices,
                             ref aSideNormals, ref bSideNormals, ref aSideUVs, ref bSideUVs, ref aSideIndices, ref bSideIndices,
                             ref createdVertices, ref createdNormals, ref createdUVs);
            }
        }
    }

    internal static void ClipTriangle(List<Vector3> triVerts, List<Vector3> triNorms, List<Vector2> triUVs, float[] triSides,
        Vector3 sliceNormal, Vector3 slicePoint, ref List<Vector3> aSideVertices, ref List<Vector3> bSideVertices,
        ref List<Vector3> aSideNormals, ref List<Vector3> bSideNormals, ref List<Vector2> aSideUVs, ref List<Vector2> bSideUVs,
        ref List<int> aSideIndices, ref List<int> bSideIndices, ref List<Vector3> createdVertices, ref List<Vector3> createdNormals, ref List<Vector2> createdUVs)
    {
        List<Vector3> onPlaneVerts = new List<Vector3>();
        List<Vector3> onPlaneNorms = new List<Vector3>();
        List<Vector2> onPlaneUVs = new List<Vector2>();

        for (int i = 0; i < 3; i++)
        {
            int prevIndex = (i + 2) % 3;

            if (triSides[i] * triSides[prevIndex] < 0)
            {
                float t = triSides[i] / (triSides[i] - triSides[prevIndex]);
                Vector3 planeVert = Vector3.Lerp(triVerts[i], triVerts[prevIndex], t);
                Vector3 planeNorm = Vector3.Lerp(triNorms[i], triNorms[prevIndex], t);
                Vector2 planeUV = Vector2.Lerp(triUVs[i], triUVs[prevIndex], t);

                onPlaneVerts.Add(planeVert);
                onPlaneNorms.Add(planeNorm);
                onPlaneUVs.Add(planeUV);
            }

            if (triSides[i] > 0)
            {
                aSideVertices.Add(triVerts[i]);
                aSideNormals.Add(triNorms[i]);
                aSideUVs.Add(triUVs[i]);
                aSideIndices.Add(aSideVertices.Count - 1);
            }
            else
            {
                bSideVertices.Add(triVerts[i]);
                bSideNormals.Add(triNorms[i]);
                bSideUVs.Add(triUVs[i]);
                bSideIndices.Add(bSideVertices.Count - 1);
            }
        }

        createdVertices.AddRange(onPlaneVerts);
        createdNormals.AddRange(onPlaneNorms);
        createdUVs.AddRange(onPlaneUVs);
    }

    internal static void MakeCap(Vector3 sliceNormal, List<Vector3> capVertices, out List<Vector3> aSideVertices, out List<Vector3> bSideVertices,
        out List<Vector3> aSideNormals, out List<Vector3> bSideNormals, out List<Vector2> aSideUVs, out List<Vector2> bSideUVs,
        out List<int> aSideIndices, out List<int> bSideIndices)
    {
        aSideVertices = new List<Vector3>();
        bSideVertices = new List<Vector3>();
        aSideNormals = new List<Vector3>();
        bSideNormals = new List<Vector3>();
        aSideUVs = new List<Vector2>();
        bSideUVs = new List<Vector2>();
        aSideIndices = new List<int>();
        bSideIndices = new List<int>();

        for (int i = 0; i < capVertices.Count; i += 2)
        {
            aSideVertices.Add(capVertices[i]);
            aSideVertices.Add(capVertices[i + 1]);
            aSideNormals.Add(sliceNormal);
            aSideNormals.Add(sliceNormal);
            aSideUVs.Add(Vector2.zero);
            aSideUVs.Add(Vector2.zero);

            bSideVertices.Add(capVertices[i + 1]);
            bSideVertices.Add(capVertices[i]);
            bSideNormals.Add(-sliceNormal);
            bSideNormals.Add(-sliceNormal);
            bSideUVs.Add(Vector2.zero);
            bSideUVs.Add(Vector2.zero);
        }

        for (int i = 0; i < aSideVertices.Count; i += 2)
        {
            aSideIndices.Add(i);
            aSideIndices.Add(i + 1);
            aSideIndices.Add((i + 2) % aSideVertices.Count);
            aSideIndices.Add((i + 2) % aSideVertices.Count);
            aSideIndices.Add(i + 1);
            aSideIndices.Add((i + 3) % aSideVertices.Count);
        }

        for (int i = 0; i < bSideVertices.Count; i += 2)
        {
            bSideIndices.Add(i);
            bSideIndices.Add(i + 1);
            bSideIndices.Add((i + 2) % bSideVertices.Count);
            bSideIndices.Add((i + 2) % bSideVertices.Count);
            bSideIndices.Add(i + 1);
            bSideIndices.Add((i + 3) % bSideVertices.Count);
        }
    }
}
