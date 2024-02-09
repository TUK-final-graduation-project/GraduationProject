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
        waveUI.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (waveMgr.waveNum == 1)
        {
            this.waveUI.SetActive(true);
        }
        this.waveUI.GetComponent<Text>().text = "WAVE " + waveMgr.waveNum;
        this.timeUI.GetComponent<Text>().text = "다음 WAVE까지 남은 시간 " + waveMgr.gameTime.ToString("F1");
        if ( waveMgr.waveNum == 10)
        {
            this.timeUI.SetActive(false);
        }
    }
}
