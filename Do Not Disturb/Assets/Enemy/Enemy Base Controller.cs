using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseController : MonoBehaviour
{
    [Header("UNIT")]
    [SerializeField] GameObject Unit;
    [SerializeField] Transform startPosition;
    [SerializeField] float minionSpeed;
    [SerializeField] float curTime;
    [SerializeField] float maxTime;
    int waveLevel;
    int unitGenerateNum;
    bool isRunning = true;

    private void Update()
    {
        curTime -= Time.deltaTime;
        if (curTime <= 0 && isRunning)
        {
            var a = Instantiate(Unit, startPosition.position, startPosition.rotation);
            a.GetComponent<UnitMove>().target = GameObject.Find("player").transform.position;
            curTime = maxTime;
            unitGenerateNum++;
            isRunning = false;
            if (unitGenerateNum > waveLevel)
            {
                isRunning = false;
            }
        }
    }

    public void UnitGenerator(int waveLv)
    {
        // wave 당 정해진 숫자의 unit만 생성
        // unit 생성 속도 조정
        // unit 생성 수 조정
        unitGenerateNum = 0;
        waveLevel = waveLv;
        curTime = maxTime;
        isRunning = true;
    }
}
