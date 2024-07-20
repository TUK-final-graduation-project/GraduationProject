using UnityEngine;
using UnityEngine.UI;
using static Unity.PlasticSCM.Editor.WebApi.CredentialsResponse;

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
    public Tower tower;

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
    public void UpgradePlayerSpeed(int coinNum)
    {
        laboratory.UpgradePlayerSpeed(coinNum);
    }

    public void UpgradePlayerHP(int coinNum)
    {
        laboratory.UpgradePlayerHP(coinNum);
    }

    public void UpgradePlayerDamage(int coinNum)
    {
        laboratory.UpgradePlayerDamage(coinNum);
    }

    public void UpgradeBase()
    {
        laboratory.UpgradeUserBase();
    }

    public void UpgradeTowerBuildSpeed(int speed)
    {
        laboratory.UpgradeTowerBuildSpeed(tower, speed);
    }

    public void UpgradeTowerHP(int hp)
    {
        laboratory.UpgradeTowerHP(tower, hp);
    }


    public void UpgradeTowerDef(int def)
    {
        laboratory.UpgradeTowerDef(tower, def);
    }

    public void UpgradeTowerAttackSpeed(int attSpeed)
    {
        laboratory.UpgradeTowerAttackSpeed(tower, attSpeed);
    }


    // Resource
    public void UpgradeResourceRespawnSpeed(int coinNum)
    {
        laboratory.UpgradeResourceRespawnSpeed(coinNum);

    }
}
