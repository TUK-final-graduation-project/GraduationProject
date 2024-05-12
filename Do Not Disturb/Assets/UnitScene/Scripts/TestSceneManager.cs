using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestSceneManager : MonoBehaviour
{
    public bool isSpawnUnit;
    public bool vDown;

    public ComSpawnPoint fireLandMark;

    void Update()
    {
        vDown = Input.GetButton("Debug SpawnUnit");
        if (vDown && !isSpawnUnit)
        {
            Debug.Log("press button");
            fireLandMark.StartSpawn(1);
            isSpawnUnit = true;

            Invoke("SpawnCoolTime", 3);
        }
    }

    void SpawnCoolTime()
    {
        isSpawnUnit = false;
    }
}
