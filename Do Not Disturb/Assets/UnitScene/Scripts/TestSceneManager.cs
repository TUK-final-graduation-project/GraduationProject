using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestSceneManager : MonoBehaviour
{
    public bool isSpawnUnit;
    public bool isAstar;
    public bool vDown;
    public bool cDown;

    public ComSpawnPoint fireLandMark;

    void Update()
    {
        vDown = Input.GetButton("Debug SpawnUnit");
        cDown = Input.GetButton("Debug Astar");
        if (vDown && !isSpawnUnit)
        {
            Debug.Log("press v button");
            fireLandMark.StartSpawn(1);
            isSpawnUnit = true;

            Invoke("SpawnCoolTime", 3);
        }
        if ( cDown && !isAstar )
        {
            Debug.Log("on c button");
            isAstar = true;
        }
    }

    void SpawnCoolTime()
    {
        isSpawnUnit = false;
    }
}
