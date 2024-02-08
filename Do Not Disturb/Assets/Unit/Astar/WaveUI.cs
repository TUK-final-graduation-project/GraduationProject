using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveUI : MonoBehaviour
{
    public WaveManager waveMgr;
    GameObject waveUI;
    GameObject timeUI;
    void Start()
    {
        waveUI = GameObject.Find("wave");
        timeUI = GameObject.Find("time");
    }

    // Update is called once per frame
    void Update()
    {
        this.waveUI.GetComponent<Text>().text = "WAVE " + waveMgr.waveNum;
        this.timeUI.GetComponent<Text>().text = "���� WAVE���� ���� �ð� " + waveMgr.gameTime.ToString("F1");
    }
}
