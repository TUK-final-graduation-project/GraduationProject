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
            // Wave n번 UI 출력
            if (gameTime <= 0f)
            {
                waveNum += 1;
                Debug.Log("wave "+waveNum);
            
                gameTime = waveCoolTime;

                // 각 base의 Unit 생성 및 player 향해 이동
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
