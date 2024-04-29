using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyBaseController : MonoBehaviour
{
    [Header("UNIT")]
    [SerializeField] GameObject Unit;
    [SerializeField] float unitCreateCurTime;
    [SerializeField] float unitCreateCoolTime;

    int waveLevel;
    int unitGenerateNum;
    bool isRunning = true;

    Transform startPosition;

    private void Start()
    {
        startPosition = transform;   
    }
    private void Update()
    {
        unitCreateCurTime -= Time.deltaTime;
        if (unitCreateCurTime <= 0 && isRunning)
        {
            var a = Instantiate(Unit, startPosition.position, startPosition.rotation);
            a.GetComponent<UnitMove>().target = GameObject.Find("Player").transform.position;
            unitCreateCurTime = unitCreateCoolTime;
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
        unitCreateCurTime = unitCreateCoolTime;
        isRunning = true;
    }
}
