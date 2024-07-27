using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VIdeoPlay : MonoBehaviour
{
    [SerializeField]
    private GameObject VideoObject;

    void Start()
    {
        Hide();
    }

    public void Show()
    {
        VideoObject.SetActive(true);
    }

    public void Hide()
    {
        VideoObject.SetActive(false);
    }
    

}
