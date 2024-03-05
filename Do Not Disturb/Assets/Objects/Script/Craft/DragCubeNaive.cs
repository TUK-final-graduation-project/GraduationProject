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
        dist = Vector3.zero; // 마우스를 클릭을 끝내면 영벡터로 초기화 한다.
    }

    private void OnMouseDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Floor")))
        {
            if (dist == Vector3.zero) // 영벡터이면 dist를 계산한다.
            {
                dist = hit.point - transform.position;
            }

            transform.position = hit.point - dist; // dist를 빼서 위치를 보정한다.
        }
    }
}