using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestSceneManager : MonoBehaviour
{
    public bool isSpawnUnit;
    public bool isAstar = false;
    public bool vDown;
    public bool cDown;
    bool xDown;

    public ComSpawnPoint fireLandMark;
    public GameObject obs;

    void Update()
    {
        vDown = Input.GetButton("Debug SpawnUnit");
        cDown = Input.GetButton("Debug Astar");
        xDown = Input.GetButton("Debug offObs");
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
        if (xDown)
        {
            obs.SetActive(false);
        }
        if (cDown)
        {
            obs.SetActive(true);
        }
    }

    void SpawnCoolTime()
    {
        isSpawnUnit = false;
    }
}
