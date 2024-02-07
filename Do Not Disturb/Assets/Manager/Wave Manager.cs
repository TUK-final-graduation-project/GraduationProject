using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public float gameTime;
    public int waveNum = 0;
    public EnemyBaseController enemyBase;

    void Update()
    {
        gameTime += Time.deltaTime;
        // Wave n�� UI ���
        if (gameTime > 2f && waveNum == 0)
        {
            waveNum += 1;
            Debug.Log("wave "+waveNum);
            // gameTime = 0f;
            // Unit ���� �� player ���� �̵�
            enemyBase.UnitGenerator(waveNum);
        }
    }
}
