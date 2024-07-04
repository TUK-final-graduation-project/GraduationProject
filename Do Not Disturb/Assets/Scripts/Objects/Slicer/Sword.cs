using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    // 프레임당 생성할 정점 수
    private const int NUM_VERTICES = 12;

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
    public bool DestroySliced;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Vector3 preTipPos;
    private Vector3 preBasePos;
    private Vector3 triggerEnterTipPos;
    private Vector3 triggerEnterBasePos;
    private Vector3 triggerExitTipPos;

    void Start()
    {

        vertices = new Vector3[NUM_VERTICES];
        triangles = new int[vertices.Length];

        // 끝과 기준 위치 설정
        preTipPos = tip.transform.position;
        preBasePos = swordBase.transform.position;

    }

    private void OnTriggerEnter(Collider other)
    {
        triggerEnterTipPos = tip.transform.position;
        triggerEnterBasePos = swordBase.transform.position;
    }

    private void OnTriggerExit(Collider other)
    {
        triggerExitTipPos = tip.transform.position;

        // 팁과 베이스 사이에 삼각형을 만들어서 노말을 얻는다.
        Vector3 side1 = triggerExitTipPos - triggerEnterTipPos;
        Vector3 side2 = triggerExitTipPos - triggerEnterBasePos;

        // 삼각형 위의 점을 얻어 노말을 계산
        Vector3 normal = Vector3.Cross(side1, side2).normalized;

        // 노말을 자르려는 객체의 로컬 변환(transform)과 일치하도록 변환
        Vector3 transformedNormal = ((Vector3)(other.gameObject.transform.localToWorldMatrix.transpose * normal)).normalized;

        // 자르려는 객체의 로컬 변환에서 시작점을 얻는다.
        Vector3 transformedStartingPoint = other.gameObject.transform.InverseTransformPoint(triggerEnterTipPos);

        Plane plane = new Plane();

        plane.SetNormalAndPosition(
                transformedNormal,
                transformedStartingPoint);

        var direction = Vector3.Dot(Vector3.up, transformedNormal);

        // 양쪽의 메시가 어느 쪽에 있는지 항상 알 수 있도록 평면을 뒤집음.

        if (direction < 0)
        {
            plane = plane.flipped;
        }

        GameObject[] slices = Slicer.Slice(plane, other.gameObject);
        Destroy(other.gameObject);

        Rigidbody rigidbody = slices[1].GetComponent<Rigidbody>();
        Vector3 newNormal = transformedNormal + Vector3.up * cutForce;
        rigidbody.AddForce(newNormal, ForceMode.Impulse);

        // 3초 후 객체 삭제
        Destroy(slices[0], 3);
        Destroy(slices[1], 3);
    }
}
