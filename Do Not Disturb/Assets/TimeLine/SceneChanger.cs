using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public float ChangeTime;
    public int SceneNumber;

    void Update()
    {
        ChangeTime -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SceneManager.LoadScene(SceneNumber);
        }

        if (ChangeTime <= 0)
        {
            Debug.Log("¾À º¯°æ");
            if (SceneNumber == 2 || SceneNumber == 3)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            SceneManager.LoadScene(SceneNumber);
        }
    }
}
