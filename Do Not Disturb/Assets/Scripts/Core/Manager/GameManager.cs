using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum States { ready, battle, gameEnd };

    public int stage;
    public int maxStage;
    public States state;
    public int enemyCnt;

    public float battleTime;
    public float readyTime;
    public float playTime;

    public float coolTimeOfReady;
    public float coolTimeOfBattle;

    public GameObject menuPanel;
    public GameObject gamePanel;

    public Text stageTxt;
    public Text playTimeTxt;

    public Text enemyCntTxt;

    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;

    public ComSpawnPoint[] spawns;

    public Laboratory laboratory;

    float uiTime;
    private void Awake()
    {
        state = States.ready;
        uiTime = readyTime;
        readyTime = coolTimeOfReady;
        battleTime = coolTimeOfBattle;
    }
    private void Update()
    {
        if (state == States.ready)
        {
            Ready();
        }
        else if (state == States.battle)
        {
            Battle();
        }
        playTime += Time.deltaTime;
    }
    void Ready()
    {
        readyTime -= Time.deltaTime;
        uiTime = readyTime;

        if (readyTime < 0)
        {
            state = States.battle;
            readyTime = coolTimeOfReady;
        }
    }

    void Battle()
    {
        battleTime -= Time.deltaTime;
        uiTime = battleTime;

        if (battleTime < 0)
        {
            state = States.ready;
            battleTime = coolTimeOfBattle;
            stage += 1;
            foreach (ComSpawnPoint spawn in spawns)
            {
                spawn.StartSpawn(stage + 1);
            }
            if (stage == (maxStage + 1))
            {
                stage = maxStage;
                state = States.gameEnd;
            }
        }
    }
    private void LateUpdate()
    {
        // LateUpdate: Update()가 끝난 후 호출되는 생명주기

        int hour = (int)(uiTime / 3600);
        int min = (int)((uiTime - hour * 3600) / 60);
        int second = (int)(uiTime % 60);

        if (state == States.ready)
        {
            stageTxt.text = "READY TO " + stage;
        }
        else
        {
            stageTxt.text = "STAGE " + stage;
        }
        playTimeTxt.text = string.Format("{0:00}", hour) + ":" + string.Format("{0:00}", min) + ":" + string.Format("{0:00}", second);

        enemyCntTxt.text = "X " + enemyCnt.ToString();
    }


    // 업그레이드 관련
    public void UpgradePlayerSpeed()
    {
        laboratory.UpgradeSpeed();
    }

    public void UpgradePlayerHP()
    {
        laboratory.UpgradeHP();
    }

    public void UpgradePlayerDamage()
    {
        laboratory.UpgradeDamage();
    }

    public void UpgradeBase()
    {
        laboratory.UpgradeBase();
    }

    public void UpgradePlayerDamage()
    {
        laboratory.UpgradeTower();
    }
}
