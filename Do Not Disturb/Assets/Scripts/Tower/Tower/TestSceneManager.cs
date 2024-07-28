using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TestSceneManager : MonoBehaviour
{

    bool bDown;
    bool nDown;
    bool mDown;
    bool escDown;

    int lobbyScene = 0;
    int gameScene = 1;
    int optionScene = 2;
    int CinematicScene_Start = 4;
    void Update()
    {
        bDown = Input.GetButton("Game Scene");
        nDown = Input.GetButton("Mini Scene");
        mDown = Input.GetButton("Lobby Scene");
        escDown = Input.GetButton("Game End");

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

        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            SceneManager.LoadScene(gameScene);
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
}
