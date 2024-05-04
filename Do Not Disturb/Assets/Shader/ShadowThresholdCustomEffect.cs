using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]

public class ShadowThresholdCustomEffect : MonoBehaviour
{
    public Material shadowMaterial;
    public Color shadowColor;
    [Range(0,1)]
    public float shadowThreshold;

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        shadowMaterial.SetFloat("_ShadowThreshold", shadowThreshold);
        shadowMaterial.SetColor("_ShadowColor", shadowColor);
        if (shadowMaterial != null)
        {
            Graphics.Blit(source, destination, shadowMaterial);
        }
        else
        {
            Debug.Log("Material이 설정되지 않았습니다.");
        }
    }
}
