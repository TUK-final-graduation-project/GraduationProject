using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipScript : MonoBehaviour
{
    public GameManager manager;
    public GameObject home;
    bool xDown;
    bool cDown;
    bool vDown;
    bool zDown;
    public GameObject snowman;
    bool onetime;
    bool vOnetime;
    bool xOnetime;
    bool zOnetime;
    private void Update()
    {
        xDown = Input.GetButton("SkipToRockBoss");
        cDown = Input.GetButton("SkipToUserBaseDestroy");
        vDown = Input.GetButton("SkipToUnitSpawn");
        zDown = Input.GetButton("SkipToWin");
        
        
        if (xDown && !xOnetime)
        {
            manager.state = GameManager.States.ready;
            manager.stage = 4;
            manager.readyTime = -1f;
            xOnetime = true;
        }

        if (cDown && !onetime)
        {
            home.GetComponent<UserHome>().baseHP = 10;
            GameObject _iUnit = Instantiate(snowman, transform.position + Vector3.forward * 30f, Quaternion.identity);
            _iUnit.GetComponent<EnemyUnitController>().target = home;
            _iUnit.GetComponent<EnemyUnitController>().Base = home;
            onetime = true;
        }

        if (vDown && !vOnetime)
        {
            manager.state = GameManager.States.ready;
            manager.stage = 1;
            manager.readyTime = -1f;
            vOnetime = true;
        }
        if (zDown && !zOnetime)
        {
            SceneManager.LoadScene(5);
            zOnetime = true;
        }
    }
}
