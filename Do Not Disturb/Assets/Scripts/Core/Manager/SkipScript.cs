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
    public GameObject snowman;
    bool onetime;
    private void Update()
    {
        xDown = Input.GetButton("SkipToRockBoss");
        cDown = Input.GetButton("SkipToUserBaseDestroy");
        
        
        if (xDown)
        {
            manager.state = GameManager.States.ready;
            manager.stage = 4;
            manager.readyTime = -1f;
        }

        if (cDown && !onetime)
        {
            home.GetComponent<UserHome>().baseHP = 10;
            GameObject _iUnit = Instantiate(snowman, transform.position + Vector3.forward * 30f, Quaternion.identity);
            _iUnit.GetComponent<EnemyUnitController>().target = home;
            _iUnit.GetComponent<EnemyUnitController>().Base = home;
            onetime = true;
        }
    }
}
