using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public float gameTime;
    public float waveCoolTime;
    public int waveNum = 0;
    public EnemyBaseController[] enemyBase;

    private void Start()
    {
        enemyBase = FindObjectsOfType(typeof(EnemyBaseController)) as EnemyBaseController[];
        gameTime = waveCoolTime;
    }
    void Update()
    {
        if (waveNum < 10)
        {
            // Wave n�� UI ���
            if (gameTime <= 0f)
            {
                waveNum += 1;
                Debug.Log("wave "+waveNum);
            
                gameTime = waveCoolTime;

                // �� base�� Unit ���� �� player ���� �̵�
                foreach (EnemyBaseController b in enemyBase)
                    b.UnitGenerator(waveNum);
            }
            else
            {
                gameTime -= Time.deltaTime;
            }
        }
    }
}
