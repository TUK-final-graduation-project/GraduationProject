using UnityEngine;
using UnityEngine.SceneManagement;
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
    public Tower tower;
    public bool isPaused;

    public GameObject settingsPanel; // 설정 UI 패널

    private CraftMenu craftMenu; // CraftMenu 클래스 인스턴스 변수 추가
    float uiTime;
    private void Awake()
    {
        state = States.ready;
        uiTime = readyTime;
        readyTime = coolTimeOfReady;
        battleTime = coolTimeOfBattle;
        isPaused = false;

        // CraftMenu 인스턴스 초기화
        craftMenu = GetComponent<CraftMenu>();
        if (craftMenu == null)
        {
            Debug.LogError("CraftMenu 컴포넌트를 찾을 수 없습니다.");
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            // isPuase가 true이면 일시 정지 해제
            if (isPaused)
            {
                ResumeSetting();
            }
            // isPuase가 false이면 일시 정지
            else
            {
                PauseSetting();
            }
        }
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

    public void UpgradeDiscountItem(int coinNum)
    {
        if (craftMenu != null)
        {
            craftMenu.ApplyDiscount(coinNum); // ApplyDiscount는 CraftMenu 클래스에 구현할 메서드
        }
        else
        {
            Debug.LogError("CraftMenu 인스턴스가 없습니다.");
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

    // 게임 시간 로직
    public void PauseSetting()
    {
        Time.timeScale = 0; // 게임 일시 정지
        isPaused = true; // 상태 변경
        settingsPanel.SetActive(true); // 설정 UI 표시
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeSetting()
    {
        Time.timeScale = 1; // 게임 재개
        isPaused = false; // 상태 변경
        settingsPanel.SetActive(false); // 설정 UI 숨김
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void GoToLobby()
    {
        SceneManager.LoadScene(0);
    }
    public void GoToSetting()
    {
        //SceneManager.LoadScene(2);
    }

    public void GameQuit()
    {
        Application.Quit();
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
