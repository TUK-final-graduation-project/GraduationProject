using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

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
    public Text coinTxt;  // Text component to display coin count
    public Text actionText; // Text component to display action messages

    public RectTransform bossHealthGroup;
    public RectTransform bossHealthBar;
    float bossHealth;

    public ComSpawnPoint[] spawns;
    public GameObject[] Bosses;

    public Laboratory laboratory;
    public bool isPaused;

    public GameObject settingsPanel; // 설정 UI 패널
    public GameObject scorePanel; // 코인 UI 패널

    private CraftMenu craftMenu; // CraftMenu 클래스 인스턴스 변수 추가
    private PlayerMovement playerMovement; // Reference to PlayerMovement

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

        // Find the PlayerMovement component in the scene
        playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement 컴포넌트를 찾을 수 없습니다.");
        }

        StartCoroutine(UpdateCoins()); // Start the coroutine to update coins

        bossHealthGroup.transform.gameObject.SetActive(false);
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
        if (CalculateGameEnd())
        {
            SceneManager.LoadScene(2);
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
            if (stage == 4)
            {
                Bosses[0].SetActive(true);
                bossHealthGroup.gameObject.SetActive(true);
            }
            else if (stage == 7)
            {
                Bosses[1].SetActive(true);
                bossHealthGroup.gameObject.SetActive(true);
            }
            else if (stage == 10)
            {
                Bosses[2].SetActive(true);
                bossHealthGroup.gameObject.SetActive(true);
            }
            foreach (ComSpawnPoint spawn in spawns)
            {
                spawn.StartSpawn(stage + 1);
            }
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
            if (stage == (maxStage + 1))
            {
                stage = maxStage;
                state = States.gameEnd;

                SceneManager.LoadScene(2);
            }
        }
    }
    bool CalculateGameEnd()
    {
        foreach(ComSpawnPoint spawn in spawns)
        {
            if (!spawn.isConquer)
            {
                return false;
            }
        }
        return true;
    }
    private void LateUpdate()
    {
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

        if (playerMovement != null)
        {
            coinTxt.text = playerMovement.coinCount.ToString() + " $";
            laboratory.UpdateButtonUI();
        }
    }


    private IEnumerator UpdateCoins()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (playerMovement != null)
            {
                playerMovement.coinCount += 1;
                laboratory.UpdateButtonUI();
            }
        }
    }


    private IEnumerator ShowActionText(string message, Color color, float duration)
    {
        actionText.text = message;
        actionText.color = color;
        actionText.gameObject.SetActive(true);

        yield return new WaitForSeconds(duration);

        actionText.gameObject.SetActive(false);
    }

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


    public void UpgradePlayerSpeed()
    {
        if (playerMovement.coinCount >= laboratory.PlayerSpeedCost)
        {
            playerMovement.coinCount -= laboratory.PlayerSpeedCost;
            laboratory.UpgradePlayerSpeed();
            StartCoroutine(ShowActionText("이동속도가 증가 되었습니다!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("코인이 부족합니다!", Color.red, 2.0f));
        }
    }


    public void UpgradeBase()
    {
        if (playerMovement.coinCount >= laboratory.BaseHPCost)
        {
            playerMovement.coinCount -= laboratory.BaseHPCost;
            laboratory.UpgradeUserBase();
            StartCoroutine(ShowActionText("기지 생명력이 회복되었습니다!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("코인이 부족합니다!", Color.red, 2.0f));
        }
    }

    public void UpgradeResourceRespawnSpeed()
    {
        if (playerMovement.coinCount >= laboratory.ResourceRespawnTimeCost)
        {
            playerMovement.coinCount -= laboratory.ResourceRespawnTimeCost;
            laboratory.UpgradeResourceRespawnSpeed();
            StartCoroutine(ShowActionText("자원 생성 속도가 빨라졌습니다!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("코인이 부족합니다!", Color.red, 2.0f));
        }
    }

    public void UpgradeDiscountItem()
    {
        if (craftMenu != null)
        {
            if (playerMovement.coinCount >= laboratory.ResourceRequiredItemsCost)
            {
                playerMovement.coinCount -= laboratory.ResourceRequiredItemsCost;
                craftMenu.ApplyDiscount(0.5f); // 매개변수는 할인율 (/100)
                StartCoroutine(ShowActionText("건설 소모 아이템이 할인 되었습니다!", Color.green, 2.0f));
            }
            else
            {
                StartCoroutine(ShowActionText("코인이 부족합니다!", Color.red, 2.0f));
            }
        }
        else
        {
            Debug.LogError("CraftMenu 인스턴스가 없습니다.");
        }
    }

    public void UpgradeTowerHP()
    {
        if (playerMovement.coinCount >= laboratory.TowerHPCost)
        {
            playerMovement.coinCount -= laboratory.TowerHPCost;
            laboratory.UpgradeTowerHP();
            StartCoroutine(ShowActionText("타워 생명력이 강화되었습니다!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("코인이 부족합니다!", Color.red, 2.0f));
        }
    }

    public void UpgradeTowerAttackSpeed()
    {
        if (playerMovement.coinCount >= laboratory.TowerSpeedCost)
        {
            playerMovement.coinCount -= laboratory.TowerSpeedCost;
            laboratory.UpgradeTowerAttackSpeed();
            StartCoroutine(ShowActionText("타워 공격속도가 빨라집니다!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("코인이 부족합니다!", Color.red, 2.0f));
        }
    }

    public void UpgradeUserUnitSpeed()
    {
        if (playerMovement.coinCount >= laboratory.UserUnitHP)
        {
            playerMovement.coinCount -= laboratory.UserUnitHP;
            laboratory.UpgradeUserUnitSpeed();
            StartCoroutine(ShowActionText("아군 유닛 이동속도가 빨라집니다!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("코인이 부족합니다!", Color.red, 2.0f));
        }
    }

    public void UpgradeUserUnitDamage()
    {
        if (playerMovement.coinCount >= laboratory.UserUnitSpeed)
        {
            playerMovement.coinCount -= laboratory.UserUnitSpeed;
            laboratory.UpgradeUserUnitDamage();
            StartCoroutine(ShowActionText("아군 유닛 공격력이 강해집니다!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("코인이 부족합니다!", Color.red, 2.0f));
        }
    }
    public void UpgradeEnemyUnitSpeed()
    {
        if (playerMovement.coinCount >= laboratory.EnemyUnitHP)
        {
            playerMovement.coinCount -= laboratory.EnemyUnitHP;
            laboratory.UpgradeEnemyUnitSpeed();
            StartCoroutine(ShowActionText("적군 유닛 이동속도가 느려집니다!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("코인이 부족합니다!", Color.red, 2.0f));
        }
    }

    public void UpgradeEnemyUnitDamage()
    {
        if (playerMovement.coinCount >= laboratory.EnemyUnitSpeed)
        {
            playerMovement.coinCount -= laboratory.EnemyUnitSpeed;
            laboratory.UpgradeEnemyUnitDamage();
            StartCoroutine(ShowActionText("적군 유닛 공격력이 줄어듭니다!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("코인이 부족합니다!", Color.red, 2.0f));
        }
    }

    public void UpdateBossHP()
    {
        switch(stage)
        {
            case 4:
                {
                    bossHealth = Bosses[0].GetComponent<BossUnit>().HP / 100f;
                    Debug.Log(bossHealth);
                    bossHealthBar.transform.localScale = new Vector3(bossHealth, 1, 1);
                    break;
                }
            case 7:
                {
                    bossHealth = Bosses[1].GetComponent<BossUnit>().HP / 100f;
                    Debug.Log(bossHealth);
                    bossHealthBar.transform.localScale = new Vector3(bossHealth, 1, 1);
                    break;
                }
            case 10:
                {
                    bossHealth = Bosses[2].GetComponent<BossUnit>().HP / 100f;
                    Debug.Log(bossHealth);
                    bossHealthBar.transform.localScale = new Vector3(bossHealth, 1, 1);
                    break;
                }
        }
        if ( bossHealth <= 0 )
        {
            bossHealthGroup.gameObject.SetActive(false);
        }
    }
}
