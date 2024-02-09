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

    void Update()
    {
        if (waveMgr.gameState == WaveManager.States.ready)
        {
            this.timeUI.GetComponent<Text>().text = "다음 WAVE까지 남은 시간 " + waveMgr.readyTime.ToString("F1");
            waveUI.SetActive(false);
        }
        else if (waveMgr.gameState == WaveManager.States.wave)
        {
            this.timeUI.GetComponent<Text>().text = "웨이브 진행 중~~ " + waveMgr.waveTime.ToString("F1");
            waveUI.SetActive(true);
            this.waveUI.GetComponent<Text>().text = "WAVE " + waveMgr.waveNum;
        }
    }
}
