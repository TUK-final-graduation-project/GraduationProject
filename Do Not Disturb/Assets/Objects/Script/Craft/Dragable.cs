using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragable : MonoBehaviour
{
    RaycastHit hit, hitGround;
    GameObject dragAnchor;
    Vector3 mainCam;

    void Start()
    {
        mainCam = Camera.main.transform.position;
    }

    private void OnMouseUp()
    {
        transform.SetParent(null);
        Destroy(dragAnchor);
    }

    private void OnMouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            dragAnchor = new GameObject("DragAnchor");
            dragAnchor.transform.position = hit.point;
            transform.SetParent(dragAnchor.transform);
            Debug.Log(hit.point + " ///// ");
        }
    }

    private void OnMouseDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hitGround, Mathf.Infinity, LayerMask.GetMask("Terrain")))
        {
            float h = dragAnchor.transform.position.y;

            Vector3 camToFloor = hitGround.point - Camera.main.transform.position;
            Vector3 nextPosition = Vector3.zero;

            float lo = 0.0f, hi = 1.0f; // ∫Ò¿≤
            for (int i = 0; i < 38; i++)
            {
                float diff = hi - lo;
                float p1 = lo + diff / 3;
                float p2 = hi - diff / 3;

                var v1 = mainCam + camToFloor * p1;
                var v2 = mainCam + camToFloor * p2;
                if (Mathf.Abs(v1.y - h) > Mathf.Abs(v2.y - h))
                {
                    nextPosition = v2;
                    lo = p1;
                }
                else
                {
                    nextPosition = v1;
                    hi = p2;
                }
            }

            dragAnchor.transform.position = nextPosition;
        }
            Debug.Log(dragAnchor.transform.position + " //2 on mouse// ");
    }
}