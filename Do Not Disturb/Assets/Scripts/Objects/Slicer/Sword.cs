using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    // 프레임당 생성할 정점 수
    private const int NUM_VERTICES = 12;
    private const float SLICE_DESTROY_DELAY = 3f;

    [SerializeField]
    [Tooltip("블레이드 첨단 오브젝트")]
    private GameObject tip = null;

    [SerializeField]
    [Tooltip("블레이드 기준의 베이스 오브젝트")]
    private GameObject swordBase = null;

    [SerializeField]
    [ColorUsage(true, true)]
    [Tooltip("블레이드 색상")]
    private Color color = Color.red;

    [SerializeField]
    [Tooltip("자르기 각 부분에 적용되는 힘의 양")]
    private float cutForce = 3f;

    [SerializeField]
    public bool destroySlicedObjects = true;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;

    // 이전 프레임에서의 위치
    private Vector3 prevTipPosition;
    private Vector3 prevBasePosition;

    // 트리거 진입 및 종료 시 위치
    private Vector3 triggerEnterTipPos;
    private Vector3 triggerEnterBasePos;
    private Vector3 triggerExitTipPos;

    private void Start()
    {
        InitializeMesh();
        SetInitialPositions();
    }

    private void InitializeMesh()
    {
        vertices = new Vector3[NUM_VERTICES];
        triangles = new int[vertices.Length];
    }

    private void SetInitialPositions()
    {
        prevTipPosition = tip.transform.position;
        prevBasePosition = swordBase.transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        RecordTriggerEnterPositions();
    }

    private void OnTriggerExit(Collider other)
    {
        RecordTriggerExitPosition();
        ProcessSlice(other);
    }

    private void RecordTriggerEnterPositions()
    {
        triggerEnterTipPos = tip.transform.position;
        triggerEnterBasePos = swordBase.transform.position;
    }

    private void RecordTriggerExitPosition()
    {
        triggerExitTipPos = tip.transform.position;
    }

    private Plane CalculateSlicingPlane(Collider other)
    {
        Vector3 side1 = triggerExitTipPos - triggerEnterTipPos;
        Vector3 side2 = triggerExitTipPos - triggerEnterBasePos;

        // 노말 벡터 계산
        Vector3 normal = Vector3.Cross(side1, side2).normalized;

        // 로컬 변환으로 변환된 노말과 시작점 계산
        Vector3 transformedNormal = TransformNormalToOtherLocalSpace(normal, other);
        Vector3 transformedStartingPoint = other.gameObject.transform.InverseTransformPoint(triggerEnterTipPos);

        Plane plane = new Plane(transformedNormal, transformedStartingPoint);

        // 항상 일관된 절단 방향 유지
        if (Vector3.Dot(Vector3.up, transformedNormal) < 0)
        {
            plane = plane.flipped;
        }

        return plane;
    }

    private Vector3 TransformNormalToOtherLocalSpace(Vector3 normal, Collider other)
    {
        return ((Vector3)(other.gameObject.transform.localToWorldMatrix.transpose * normal)).normalized;
    }

    private void ProcessSlice(Collider other)
    {
        // 슬라이스 평면 계산
        Plane slicingPlane = CalculateSlicingPlane(other);

        // 원본 객체를 슬라이스
        GameObject[] slices = Slicer.Slice(slicingPlane, new List<GameObject> { other.gameObject }).ToArray();

        // 슬라이스된 객체가 제대로 반환되지 않았을 경우 처리
        if (slices == null || slices.Length < 2)
        {
            Debug.Log("Slicing failed: Invalid slice data.");
            return;
        }

        Destroy(other.gameObject);

        // 두 번째 조각 부터 물리력 적용
        for (int i = 1; i < slices.Length; i++)
        {
            ApplyCutForce(slices[i], slicingPlane.normal);
        }
        // 잘린 객체들 처리 (삭제 지연 시간)
        if (destroySlicedObjects)
        {
            for (int i = 0; i < slices.Length; i++)
            {
                Destroy(slices[i], SLICE_DESTROY_DELAY);
            }
        }
    }

    private void ApplyCutForce(GameObject slice, Vector3 normal)
    {
        // 리지드바디가 없다면, 물리력 적용을 하지 않음
        Rigidbody rigidbody = slice.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            Vector3 forceDirection = normal + Vector3.up * cutForce;
            rigidbody.AddForce(forceDirection, ForceMode.Impulse);
        }
        else
        {
            Debug.LogWarning("No Rigidbody attached to the sliced object.");
        }
    }
}
