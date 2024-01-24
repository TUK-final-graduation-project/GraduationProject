using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshSlice : MonoBehaviour
{
    public static GameObject[] SlicerWorld(GameObject _target, Vector3 _sliceNormal, Vector3 _slicePoint, Material _interial)
    {
        Vector3 localNormal = _target.transform.InverseTransformVector(_sliceNormal); //localMatrix * _sliceNormal;
        Vector3 localPoint = _target.transform.InverseTransformPoint(_slicePoint); //localMatrix * _slicePoint;
        return Slicer(_target, localNormal, localPoint, _interial);
    }

    public static GameObject[] Slicer(GameObject _target, Vector3 _sliceNormal, Vector3 _slicePoint, Material _ineterial)
    {
        //Original mesh data
        Mesh orinMesh = _target.GetComponent<MeshFilter>().sharedMesh;
        Vector3[] orinVerts = orinMesh.vertices;
        Vector3[] orinNors = orinMesh.normals;
        Vector2[] orinUvs = orinMesh.uv;
        int orinSubMeshCount = orinMesh.subMeshCount;
        Material[] orinMaterials = _target.GetComponent<MeshRenderer>().sharedMaterials;

        int existInterialMatIdx = -1;

        for (int i = 0; i < orinMaterials.Length; i++)
        {
            if (orinMaterials[i].Equals(_ineterial)) { existInterialMatIdx = i; break; }
        }

        //New mesh data
        //aSide is dot product with slice normal less than zero
        //bSide is dot product with slice normal above zero
        List<Vector3> aSideVerts = new List<Vector3>();
        List<Vector3> bSideVerts = new List<Vector3>();
        List<Vector3> aSideNors = new List<Vector3>();
        List<Vector3> bSideNors = new List<Vector3>();
        List<Vector2> aSideUvs = new List<Vector2>();
        List<Vector2> bSideUvs = new List<Vector2>();
        List<int>[] aSideTris = new List<int>[orinSubMeshCount];
        List<int>[] bSideTris = new List<int>[orinSubMeshCount];



        //Created vertices as new
        List<Vector3> createdVerts = new List<Vector3>();
        List<Vector3> createdNors = new List<Vector3>();
        List<Vector2> createdUvs = new List<Vector2>();



        for (int i = 0; i < orinSubMeshCount; i++)
        {

            int aVertCount = aSideVerts.Count;
            int bVertCount = bSideVerts.Count;

            ParseSubMesh(orinVerts, orinNors, orinUvs, orinMesh.GetTriangles(i),
                _sliceNormal, _slicePoint, ref aSideVerts, ref bSideVerts,
                ref aSideNors, ref bSideNors, ref aSideUvs, ref bSideUvs, out aSideTris[i], out bSideTris[i], ref createdVerts, ref createdNors, ref createdUvs);

            //Supplement tris data
            for (int j = 0; j < aSideTris[i].Count; j++)
            {
                aSideTris[i][j] += aVertCount;
            }

            for (int j = 0; j < bSideTris[i].Count; j++)
            {
                bSideTris[i][j] += bVertCount;
            }
        }

        //Sort and optimize created vert
        List<Vector3> sortedCreatedVerts;
        SortVertices(createdVerts, out sortedCreatedVerts);

        //Cap data
        List<Vector3> aSideCapVerts, bSideCapVerts;
        List<Vector3> aSideCapNors, bSideCapNors;
        List<Vector2> aSideCapUvs, bSideCapUvs;
        List<int> aSideCapTris, bSideCapTris;


        //Make cap
        MakeCap(_sliceNormal, sortedCreatedVerts, out aSideCapVerts, out bSideCapVerts, out aSideCapNors, out bSideCapNors, out aSideCapUvs, out bSideCapUvs, out aSideCapTris, out bSideCapTris);

        //Supplement cap data
        for (int i = 0; i < aSideCapTris.Count; i++)
        {
            aSideCapTris[i] += aSideVerts.Count;
        }
        for (int i = 0; i < bSideCapTris.Count; i++)
        {
            bSideCapTris[i] += bSideVerts.Count;
        }

        //Finalize mesh data
        List<Vector3> aSideFinalVerts = new List<Vector3>();
        List<Vector3> bSideFinalVerts = new List<Vector3>();
        List<Vector3> aSideFinalNors = new List<Vector3>();
        List<Vector3> bSideFinalNors = new List<Vector3>();
        List<Vector2> aSideFinalUvs = new List<Vector2>();
        List<Vector2> bSideFinalUvs = new List<Vector2>();

        aSideFinalVerts.AddRange(aSideVerts);
        aSideFinalVerts.AddRange(aSideCapVerts);
        bSideFinalVerts.AddRange(bSideVerts);
        bSideFinalVerts.AddRange(bSideCapVerts);
        aSideFinalNors.AddRange(aSideNors);
        aSideFinalNors.AddRange(aSideCapNors);
        bSideFinalNors.AddRange(bSideNors);
        bSideFinalNors.AddRange(bSideCapNors);
        aSideFinalUvs.AddRange(aSideUvs);
        aSideFinalUvs.AddRange(aSideCapUvs);
        bSideFinalUvs.AddRange(bSideUvs);
        bSideFinalUvs.AddRange(bSideCapUvs);

        //Use existing interial material if there have the same material.

        if (existInterialMatIdx > 0)
        {
            aSideTris[existInterialMatIdx].AddRange(aSideCapTris);
            bSideTris[existInterialMatIdx].AddRange(bSideCapTris);
        }

        //Create mesh
        Mesh aMesh = new Mesh();
        Mesh bMesh = new Mesh();
        aMesh.vertices = aSideFinalVerts.ToArray();
        aMesh.normals = aSideFinalNors.ToArray();
        aMesh.uv = aSideFinalUvs.ToArray();
        aMesh.subMeshCount = existInterialMatIdx < 0 ? orinSubMeshCount + 1 : orinSubMeshCount;

        for (int i = 0; i < orinSubMeshCount; i++)
        {
            aMesh.SetTriangles(aSideTris[i], i);
        }

        if (existInterialMatIdx < 0) aMesh.SetTriangles(aSideCapTris, orinSubMeshCount);

        bMesh.vertices = bSideFinalVerts.ToArray();
        bMesh.normals = bSideFinalNors.ToArray();
        bMesh.uv = bSideFinalUvs.ToArray();
        bMesh.subMeshCount = existInterialMatIdx < 0 ? orinSubMeshCount + 1 : orinSubMeshCount;

        for (int i = 0; i < orinSubMeshCount; i++)
        {
            bMesh.SetTriangles(bSideTris[i], i);
        }

        if (existInterialMatIdx < 0) bMesh.SetTriangles(bSideCapTris, orinSubMeshCount);

        //Create sliced object
        GameObject aObject = new GameObject(_target.name + "_A", typeof(MeshFilter), typeof(MeshRenderer));
        GameObject bObject = new GameObject(_target.name + "_B", typeof(MeshFilter), typeof(MeshRenderer));
        Material[] mats = new Material[(existInterialMatIdx < 0 ? orinSubMeshCount + 1 : orinSubMeshCount)];

        for (int i = 0; i < orinSubMeshCount; i++)
        {
            mats[i] = orinMaterials[i];
        }

        if (existInterialMatIdx < 0) mats[orinSubMeshCount] = _ineterial;
        aObject.GetComponent<MeshFilter>().sharedMesh = aMesh;
        aObject.GetComponent<MeshRenderer>().sharedMaterials = mats;
        bObject.GetComponent<MeshFilter>().sharedMesh = bMesh;
        bObject.GetComponent<MeshRenderer>().sharedMaterials = mats;
        aObject.transform.position = _target.transform.position;
        aObject.transform.rotation = _target.transform.rotation;
        aObject.transform.localScale = _target.transform.localScale;
        bObject.transform.position = _target.transform.position;
        bObject.transform.rotation = _target.transform.rotation;
        bObject.transform.localScale = _target.transform.localScale;

        //Hide original object
        _target.SetActive(false);

        //Return cutted object
        return new GameObject[] { aObject, bObject };
    }

    internal static void SwapTwoIndex<T>(ref List<T> _target, int _idx0, int _idx1)
    {
        T temp = _target[_idx1];
        _target[_idx1] = _target[_idx0];
        _target[_idx0] = temp;
    }

    internal static void SwapTwoIndexSet<T>(ref List<T> _target, int _idx00, int _idx01, int _idx10, int _idx11)
    {
        T temp0 = _target[_idx00];
        T temp1 = _target[_idx01];
        _target[_idx00] = _target[_idx10];
        _target[_idx01] = _target[_idx11];
        _target[_idx10] = temp0;
        _target[_idx11] = temp1;
    }

    internal static void SortVertices(List<Vector3> _target, out List<Vector3> _result)
    {
        _result = new List<Vector3>();
        _result.Add(_target[0]);
        _result.Add(_target[1]);

        int vertSetCount = _target.Count / 2;

        for (int i = 0; i < vertSetCount - 1; i++)
        {
            Vector3 vert0 = _target[i * 2];
            Vector3 vert1 = _target[i * 2 + 1];

            for (int j = i + 1; j < vertSetCount; j++)
            {
                Vector3 cVert0 = _target[j * 2];
                Vector3 cVert1 = _target[j * 2 + 1];

                if (vert1 == cVert0)
                {
                    _result.Add(cVert1);
                    SwapTwoIndexSet<Vector3>(ref _target, i * 2 + 2, i * 2 + 3, j * 2, j * 2 + 1);
                }

                else if (vert1 == cVert1)
                {
                    _result.Add(cVert0);
                    SwapTwoIndexSet<Vector3>(ref _target, i * 2 + 2, i * 2 + 3, j * 2 + 1, j * 2);
                }
            }
        }
        if (_result[0] == _result[_result.Count - 1]) _result.RemoveAt(_result.Count - 1);
    }


    internal static void ParseSubMesh(Vector3[] _orinVerts, Vector3[] _orinNors, Vector2[] _orinUvs, int[] _subMeshTris,

        Vector3 _sliceNormal, Vector3 _slicePoint,

        ref List<Vector3> _aSideVerts, ref List<Vector3> _bSideVerts,
        ref List<Vector3> _aSideNors, ref List<Vector3> _bSideNors,
        ref List<Vector2> _aSideUvs, ref List<Vector2> _bSideUvs,
        out List<int> _aSideTris, out List<int> _bSideTris,
        ref List<Vector3> _createdVerts, ref List<Vector3> _createdNors, ref List<Vector2> _createdUvs)

    {
        _aSideTris = new List<int>();
        _bSideTris = new List<int>();

        //Split vertices
        int triCount = _subMeshTris.Length / 3;

        for (int i = 0; i < triCount; i++)
        {
            //Target vertices
            int idx0 = i * 3;
            int idx1 = idx0 + 1;
            int idx2 = idx1 + 1;

            int vertIdx0 = _subMeshTris[idx0];
            int vertIdx1 = _subMeshTris[idx1];
            int vertIdx2 = _subMeshTris[idx2];

            Vector3 vert0 = _orinVerts[vertIdx0];
            Vector3 vert1 = _orinVerts[vertIdx1];
            Vector3 vert2 = _orinVerts[vertIdx2];
            Vector3 nor0 = _orinNors[vertIdx0];
            Vector3 nor1 = _orinNors[vertIdx1];
            Vector3 nor2 = _orinNors[vertIdx2];
            Vector2 uv0 = _orinUvs[vertIdx0];
            Vector2 uv1 = _orinUvs[vertIdx1];
            Vector2 uv2 = _orinUvs[vertIdx2];

            float dot0 = Vector3.Dot(_sliceNormal, vert0 - _slicePoint);
            float dot1 = Vector3.Dot(_sliceNormal, vert1 - _slicePoint);
            float dot2 = Vector3.Dot(_sliceNormal, vert2 - _slicePoint);

            //If all vertices are at same side

            if (dot0 < 0 && dot1 < 0 && dot2 < 0)
            {
                _aSideVerts.Add(vert0);
                _aSideVerts.Add(vert1);
                _aSideVerts.Add(vert2);
                _aSideNors.Add(nor0);
                _aSideNors.Add(nor1);
                _aSideNors.Add(nor2);
                _aSideUvs.Add(uv0);
                _aSideUvs.Add(uv1);
                _aSideUvs.Add(uv2);
                _aSideTris.Add(_aSideTris.Count);
                _aSideTris.Add(_aSideTris.Count);
                _aSideTris.Add(_aSideTris.Count);
            }

            else if (dot0 >= 0 && dot1 >= 0 && dot2 >= 0)
            {
                _bSideVerts.Add(vert0);
                _bSideVerts.Add(vert1);
                _bSideVerts.Add(vert2);
                _bSideNors.Add(nor0);
                _bSideNors.Add(nor1);
                _bSideNors.Add(nor2);
                _bSideUvs.Add(uv0);
                _bSideUvs.Add(uv1);
                _bSideUvs.Add(uv2);
                _bSideTris.Add(_bSideTris.Count);
                _bSideTris.Add(_bSideTris.Count);
                _bSideTris.Add(_bSideTris.Count);
            }
            //If not all vertices are at smae side
            else
            {
                int aloneVertIdx = Mathf.Sign(dot0) == Mathf.Sign(dot1) ? vertIdx2 : (Mathf.Sign(dot0) == Mathf.Sign(dot2) ? vertIdx1 : vertIdx0);
                int otherVertIdx0 = Mathf.Sign(dot0) == Mathf.Sign(dot1) ? vertIdx0 : (Mathf.Sign(dot0) == Mathf.Sign(dot2) ? vertIdx2 : vertIdx1);
                int otherVertIdx1 = Mathf.Sign(dot0) == Mathf.Sign(dot1) ? vertIdx1 : (Mathf.Sign(dot0) == Mathf.Sign(dot2) ? vertIdx0 : vertIdx2);

                Vector3 aloneVert = _orinVerts[aloneVertIdx];
                Vector3 otherVert0 = _orinVerts[otherVertIdx0];
                Vector3 otherVert1 = _orinVerts[otherVertIdx1];
                Vector3 aloneNor = _orinNors[aloneVertIdx];
                Vector3 otherNor0 = _orinNors[otherVertIdx0];
                Vector3 otherNor1 = _orinNors[otherVertIdx1];
                Vector2 aloneUv = _orinUvs[aloneVertIdx];
                Vector2 otherUv0 = _orinUvs[otherVertIdx0];
                Vector2 otherUv1 = _orinUvs[otherVertIdx1];

                float alone2PlaneDist = Mathf.Abs(Vector3.Dot(_sliceNormal, aloneVert - _slicePoint));
                float other02PlaneDist = Mathf.Abs(Vector3.Dot(_sliceNormal, otherVert0 - _slicePoint));
                float other12PlaneDist = Mathf.Abs(Vector3.Dot(_sliceNormal, otherVert1 - _slicePoint));
                float alone2Other0Ratio = alone2PlaneDist / (alone2PlaneDist + other02PlaneDist);
                float alone2Other1Ratio = alone2PlaneDist / (alone2PlaneDist + other12PlaneDist);

                Vector3 createdVert0 = Vector3.Lerp(aloneVert, otherVert0, alone2Other0Ratio);
                Vector3 createdVert1 = Vector3.Lerp(aloneVert, otherVert1, alone2Other1Ratio);
                Vector3 createdNor0 = Vector3.Lerp(aloneNor, otherNor0, alone2Other0Ratio);
                Vector3 createdNor1 = Vector3.Lerp(aloneNor, otherNor1, alone2Other1Ratio);
                Vector2 createdUv0 = Vector2.Lerp(aloneUv, otherUv0, alone2Other0Ratio);
                Vector2 createdUv1 = Vector2.Lerp(aloneUv, otherUv1, alone2Other1Ratio);

                _createdVerts.Add(createdVert0);
                _createdVerts.Add(createdVert1);
                _createdNors.Add(createdNor0);
                _createdNors.Add(createdNor1);
                _createdUvs.Add(createdUv0);
                _createdUvs.Add(createdUv1);

                //Distribute vertices data to both side
                float aloneSide = Vector3.Dot(_sliceNormal, aloneVert - _slicePoint);

                if (aloneSide < 0)
                {
                    //A side
                    _aSideVerts.Add(aloneVert);
                    _aSideVerts.Add(createdVert0);
                    _aSideVerts.Add(createdVert1);
                    _aSideNors.Add(aloneNor);
                    _aSideNors.Add(createdNor0);
                    _aSideNors.Add(createdNor1);
                    _aSideUvs.Add(aloneUv);
                    _aSideUvs.Add(createdUv0);
                    _aSideUvs.Add(createdUv1);
                    _aSideTris.Add(_aSideTris.Count);
                    _aSideTris.Add(_aSideTris.Count);
                    _aSideTris.Add(_aSideTris.Count);

                    //B side
                    _bSideVerts.Add(otherVert0);
                    _bSideVerts.Add(otherVert1);
                    _bSideVerts.Add(createdVert0);
                    _bSideNors.Add(otherNor0);
                    _bSideNors.Add(otherNor1);
                    _bSideNors.Add(createdNor0);
                    _bSideUvs.Add(otherUv0);
                    _bSideUvs.Add(otherUv1);
                    _bSideUvs.Add(createdUv0);
                    _bSideTris.Add(_bSideTris.Count);
                    _bSideTris.Add(_bSideTris.Count);
                    _bSideTris.Add(_bSideTris.Count);

                    _bSideVerts.Add(otherVert1);
                    _bSideVerts.Add(createdVert1);
                    _bSideVerts.Add(createdVert0);
                    _bSideNors.Add(otherNor1);
                    _bSideNors.Add(createdNor1);
                    _bSideNors.Add(createdNor0);

                    _bSideUvs.Add(otherUv1);
                    _bSideUvs.Add(createdUv1);
                    _bSideUvs.Add(createdUv0);
                    _bSideTris.Add(_bSideTris.Count);
                    _bSideTris.Add(_bSideTris.Count);
                    _bSideTris.Add(_bSideTris.Count);
                }
                else
                {
                    //B side
                    _bSideVerts.Add(aloneVert);
                    _bSideVerts.Add(createdVert0);
                    _bSideVerts.Add(createdVert1);
                    _bSideNors.Add(aloneNor);
                    _bSideNors.Add(createdNor0);
                    _bSideNors.Add(createdNor1);
                    _bSideUvs.Add(aloneUv);
                    _bSideUvs.Add(createdUv0);
                    _bSideUvs.Add(createdUv1);
                    _bSideTris.Add(_bSideTris.Count);
                    _bSideTris.Add(_bSideTris.Count);
                    _bSideTris.Add(_bSideTris.Count);

                    //A side
                    _aSideVerts.Add(otherVert0);
                    _aSideVerts.Add(otherVert1);
                    _aSideVerts.Add(createdVert0);
                    _aSideNors.Add(otherNor0);
                    _aSideNors.Add(otherNor1);
                    _aSideNors.Add(createdNor0);
                    _aSideUvs.Add(otherUv0);
                    _aSideUvs.Add(otherUv1);
                    _aSideUvs.Add(createdUv0);
                    _aSideTris.Add(_aSideTris.Count);
                    _aSideTris.Add(_aSideTris.Count);
                    _aSideTris.Add(_aSideTris.Count);

                    _aSideVerts.Add(otherVert1);
                    _aSideVerts.Add(createdVert1);
                    _aSideVerts.Add(createdVert0);
                    _aSideNors.Add(otherNor1);
                    _aSideNors.Add(createdNor1);
                    _aSideNors.Add(createdNor0);
                    _aSideUvs.Add(otherUv1);
                    _aSideUvs.Add(createdUv1);
                    _aSideUvs.Add(createdUv0);
                    _aSideTris.Add(_aSideTris.Count);
                    _aSideTris.Add(_aSideTris.Count);
                    _aSideTris.Add(_aSideTris.Count);
                }
            }
        }
    }

    internal static void MakeCap(Vector3 _faceNormal, List<Vector3> _relatedVerts,
        out List<Vector3> _aSideVerts, out List<Vector3> _bSideVerts,
        out List<Vector3> _aSideNors, out List<Vector3> _bSideNors,
        out List<Vector2> _aSideUvs, out List<Vector2> _bSideUvs,
        out List<int> _aSideTris, out List<int> _bSideTris)
    {
        _aSideVerts = new List<Vector3>();
        _bSideVerts = new List<Vector3>();
        _aSideNors = new List<Vector3>();
        _bSideNors = new List<Vector3>();
        _aSideUvs = new List<Vector2>();
        _bSideUvs = new List<Vector2>();
        _aSideTris = new List<int>();
        _bSideTris = new List<int>();
        _aSideVerts.AddRange(_relatedVerts);
        _bSideVerts.AddRange(_relatedVerts);

        if (_relatedVerts.Count < 2) return;

        //Calculate center of the cap
        Vector3 center = Vector3.zero;

        foreach (Vector3 v in _relatedVerts)
        {
            center += v;
        }
        center /= _relatedVerts.Count;

        //Add center vert to both side at last
        _aSideVerts.Add(center);
        _bSideVerts.Add(center);

        //Calculate cap data
        //Normal
        for (int i = 0; i < _aSideVerts.Count; i++)
        {
            _aSideNors.Add(_faceNormal);
            _bSideNors.Add(-_faceNormal);
        }

        //Uv
        //Basis on sliced plane
        Vector3 forward = Vector3.zero;
        forward.x = _faceNormal.y;
        forward.y = -_faceNormal.x;
        forward.z = _faceNormal.z;
        Vector3 left = Vector3.Cross(forward, _faceNormal);

        for (int i = 0; i < _relatedVerts.Count; i++)
        {
            Vector3 dir = _relatedVerts[i] - center;
            Vector2 relatedUV = Vector2.zero;
            relatedUV.x = 0.5f + Vector3.Dot(dir, left);
            relatedUV.y = 0.5f + Vector3.Dot(dir, forward);
            _aSideUvs.Add(relatedUV);
            _bSideUvs.Add(relatedUV);
        }

        _aSideUvs.Add(new Vector2(0.5f, 0.5f));
        _bSideUvs.Add(new Vector2(0.5f, 0.5f));

        //Triangle
        int centerIdx = _aSideVerts.Count - 1;
        //Check first triangle face where
        float faceDir = Vector3.Dot(_faceNormal, Vector3.Cross(_relatedVerts[0] - center, _relatedVerts[1] - _relatedVerts[0]));

        //Store tris
        for (int i = 0; i < _aSideVerts.Count - 1; i++)
        {
            int idx0 = i;
            int idx1 = (i + 1) % (_aSideVerts.Count - 1);
            if (faceDir < 0)
            {
                _aSideTris.Add(centerIdx);
                _aSideTris.Add(idx1);
                _aSideTris.Add(idx0);

                _bSideTris.Add(centerIdx);
                _bSideTris.Add(idx0);
                _bSideTris.Add(idx1);
            }
            else
            {
                _aSideTris.Add(centerIdx);
                _aSideTris.Add(idx0);
                _aSideTris.Add(idx1);

                _bSideTris.Add(centerIdx);
                _bSideTris.Add(idx1);
                _bSideTris.Add(idx0);
            }
        }
    }
}