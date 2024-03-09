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
        // wave �� ������ ������ unit�� ����
        // unit ���� �ӵ� ����
        // unit ���� �� ����
        unitGenerateNum = 0;
        waveLevel = waveLv;
        curTime = maxTime;
        isRunning = true;
    }
}
