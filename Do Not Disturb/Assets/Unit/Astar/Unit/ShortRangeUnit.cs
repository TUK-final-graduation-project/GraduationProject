using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortRangeUnit : MonoBehaviour
{
    float range = 5f;
    public Material detectedMat;
    public Collider targetUnit = null;
    public Collider[] colliders;
    void changeMaterial(GameObject go, Material changeMat)
    {
        Renderer rd = go.GetComponent<MeshRenderer>();
        Material[] mat = rd.sharedMaterials;
        mat[0] = changeMat;
        rd.materials = mat;
    }

    void Update()
    {
        colliders = Physics.OverlapSphere(transform.position, range);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject != gameObject && collider.gameObject.tag == "Fire" && targetUnit == null)
            {
                targetUnit = collider;
                changeMaterial(collider.gameObject, detectedMat);
                gameObject.GetComponent<UnitMove>().StopCoroutine("FollowPath");
                Debug.Log(gameObject.name);
            }
        }
    }
}
