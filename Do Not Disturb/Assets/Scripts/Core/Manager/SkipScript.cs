using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipScript : MonoBehaviour
{
    public GameManager manager;
    public GameObject home;
    bool xDown;
    bool lDown;
    bool kDown;
    bool cDown;
    bool vDown;
    bool zDown;
    public GameObject snowman;
    bool onetime;
    bool vOnetime;
    bool xOnetime;
    bool kOnetime;
    bool lOnetime;
    bool zOnetime;
    private void Update()
    {
        xDown = Input.GetButton("SkipToRockBoss");
        lDown = Input.GetButton("SkipToWizardBoss");
        kDown = Input.GetButton("SkipToOakBoss");
        cDown = Input.GetButton("SkipToUserBaseDestroy");
        vDown = Input.GetButton("SkipToUnitSpawn");
        zDown = Input.GetButton("SkipToWin");
        // r 눌러서 돋보기
        // 1번 2번 3번 헷갈리지 말기 ^___^
        
        
        if (xDown && !xOnetime)
        {
            manager.state = GameManager.States.ready;
            manager.stage = 4;
            manager.readyTime = -1f;
            xOnetime = true;
        }
        if (kDown && !kOnetime)
        {
            manager.state = GameManager.States.ready;
            manager.stage = 10;
            manager.readyTime = -1f;
            kOnetime = true;
        }
        if (lDown && !lOnetime)
        {
            manager.state = GameManager.States.ready;
            manager.stage = 7;
            manager.readyTime = -1f;
            lOnetime = true;
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
