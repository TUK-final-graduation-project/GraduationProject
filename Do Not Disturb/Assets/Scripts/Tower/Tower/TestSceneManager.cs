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

    int lobbyScene = 0;
    int gameScene = 1;
    int optionScene = 2;
    int CinematicScene_Start = 3;
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
            SceneManager.LoadScene(CinematicScene_Start);
        }
        if (nDown)
        {
            SceneManager.LoadScene(optionScene);
        }
        if (mDown)
        {
            SceneManager.LoadScene(lobbyScene);
        }

        
    }

    public void OnApplicationQuit()
    {
        Application.Quit();
    }
    public void OnGameSceneStart()
    {
        SceneManager.LoadScene(CinematicScene_Start);
    }
    public void OnOptionSceneStart()
    {
        SceneManager.LoadScene(optionScene);
    }
    public void OnLobbySceneStart()
    {
        // 마우스 커서 On
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
        SceneManager.LoadScene(lobbyScene);
    }
    

    void SpawnCoolTime()
    {
        isSpawnUnit = false;
    }
}
