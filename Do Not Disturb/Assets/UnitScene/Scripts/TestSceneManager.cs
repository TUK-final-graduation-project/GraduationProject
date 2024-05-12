using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestSceneManager : MonoBehaviour
{
    public bool isSpawnUnit;
    public bool isAstar = false;
    public bool vDown;
    public bool cDown;
    bool xDown;
    bool bDown;
    bool nDown;
    bool mDown;
    bool escDown;

    public ComSpawnPoint fireLandMark;
    public GameObject obs;

    int lobbyScene = 2;
    int gameScene = 0;
    int miniScene = 1;
    void Update()
    {
        vDown = Input.GetButton("Debug SpawnUnit");
        cDown = Input.GetButton("Debug Astar");
        xDown = Input.GetButton("Debug offObs");
        bDown = Input.GetButton("Game Scene");
        nDown = Input.GetButton("Mini Scene");
        mDown = Input.GetButton("Lobby Scene");
        escDown = Input.GetButton("Game End");

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
        if (bDown)
        {
            SceneManager.LoadScene(gameScene);
        }
        if (nDown)
        {
            SceneManager.LoadScene(miniScene);
        }
        if (mDown)
        {
            SceneManager.LoadScene(lobbyScene);
        }

        if(escDown)
        {
            Application.Quit();
        }
    }

    void SpawnCoolTime()
    {
        isSpawnUnit = false;
    }
}
