using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragCubeNaive : MonoBehaviour
{
    RaycastHit hit;
    Vector3 dist;

    void Awake()
    {
        dist = Vector3.zero;
    }

    private void OnMouseUp()
    {
        dist = Vector3.zero; // ���콺�� Ŭ���� ������ �����ͷ� �ʱ�ȭ �Ѵ�.
    }

    private void OnMouseDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Floor")))
        {
            if (dist == Vector3.zero) // �������̸� dist�� ����Ѵ�.
            {
                dist = hit.point - transform.position;
            }

            transform.position = hit.point - dist; // dist�� ���� ��ġ�� �����Ѵ�.
        }
    }
}