using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public enum States { ready, wave, end};
    public States gameState = States.ready;

    // game play time
    public float readyTime;
    public float waveTime;

    // ���� wave�� ���� �غ��ϴ� �ð�
    public float readyCoolTime;
    public float waveCoolTime;

    // current wave
    public int waveNum = 1;
    int maxWave = 1;

    // enemy base object list
    public EnemyBaseController[] enemyBase;

    private void Start()
    {
        enemyBase = FindObjectsOfType(typeof(EnemyBaseController)) as EnemyBaseController[];
        readyTime = readyCoolTime;
        waveTime = waveCoolTime;
    }
    void Update()
    {
        if (waveNum <= maxWave)
        {
            Test();
        }
    }

    void Test()
    {
        switch (gameState)
        {
            case States.ready:
                {
                    readyTime -= Time.deltaTime;
                    if (readyTime < 0f)
                    {
                        readyTime = readyCoolTime;
                        gameState = States.wave;

                        // �� base�� Unit ���� �� player ���� �̵�
                        foreach (EnemyBaseController b in enemyBase)
                            b.UnitGenerator(waveNum);
                    }
                    break;
                }
            case States.wave:
                {
                    waveTime -= Time.deltaTime;
                    if (waveTime < 0f)
                    {
                        waveTime = waveCoolTime;
                        gameState = States.ready;
                        waveNum += 1;
                    }
                    break;
                }
        }
    }
}
