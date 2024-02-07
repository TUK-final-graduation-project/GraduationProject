using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public float gameTime;
    public int waveNum = 0;
    public EnemyBaseController[] enemyBase;

    private void Start()
    {
        enemyBase = FindObjectsOfType(typeof(EnemyBaseController)) as EnemyBaseController[];
    }
    void Update()
    {
        gameTime += Time.deltaTime;
        // Wave n�� UI ���
        if (gameTime > 2f && waveNum == 0)
        {
            waveNum += 1;
            Debug.Log("wave "+waveNum);
            
            // gameTime = 0f;

            // �� base�� Unit ���� �� player ���� �̵�
            foreach (EnemyBaseController b in enemyBase)
                b.UnitGenerator(waveNum);
        }
    }
}
