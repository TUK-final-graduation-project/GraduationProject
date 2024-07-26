using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Open : MonoBehaviour
{
    [SerializeField]
    GameObject MovieWindow;
    void Start()
    {
        HideImage(); 
    }

    public void ShowImage()
    {
        MovieWindow.SetActive(true);
    }

    public void HideImage()
    {
        MovieWindow.SetActive(false); 
    }
}
