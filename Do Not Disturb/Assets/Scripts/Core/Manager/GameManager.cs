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

    public ComSpawnPoint[] spawns;

    public Laboratory laboratory;
    public Tower tower;
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
            coinTxt.text = "Coins: " + playerMovement.coinCount.ToString(); // Update the coin count in UI
        }
    }

    private IEnumerator UpdateCoins()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f); // Wait for 1 second
            if (playerMovement != null)
            {
                playerMovement.coinCount += 1; // Increase player's coin count
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

    public void UpgradeDiscountItem(int coinNum)
    {
        if (craftMenu != null)
        {
            if (playerMovement.coinCount >= coinNum)
            {
                playerMovement.coinCount -= coinNum;
                craftMenu.ApplyDiscount(coinNum); // ApplyDiscount는 CraftMenu 클래스에 구현할 메서드
                StartCoroutine(ShowActionText("건설 아이템이 할인 되었습니다!", Color.green, 2.0f));
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

    public void UpgradePlayerSpeed(int coinNum)
    {
        if (playerMovement.coinCount >= coinNum)
        {
            playerMovement.coinCount -= coinNum;
            laboratory.UpgradePlayerSpeed();
            StartCoroutine(ShowActionText("이동속도가 증가 되었습니다!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("코인이 부족합니다!", Color.red, 2.0f));
        }
    }

    public void UpgradePlayerHP(int coinNum)
    {
        if (playerMovement.coinCount >= coinNum)
        {
            playerMovement.coinCount -= coinNum;
            laboratory.UpgradePlayerHP();
            StartCoroutine(ShowActionText("플레이어의 체력이 강화되었습니다!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("코인이 부족합니다!", Color.red, 2.0f));
        }
    }

    public void UpgradePlayerDamage(int coinNum)
    {
        if (playerMovement.coinCount >= coinNum)
        {
            playerMovement.coinCount -= coinNum;
            laboratory.UpgradePlayerDamage();
            StartCoroutine(ShowActionText("플레이어 공격력이 강화되었습니다!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("코인이 부족합니다!", Color.red, 2.0f));
        }
    }

    public void UpgradeBase(int coinNum)
    {
        if (playerMovement.coinCount >= coinNum)
        {
            playerMovement.coinCount -= coinNum;
            laboratory.UpgradeUserBase();
            StartCoroutine(ShowActionText("기지 생명력이 회복되었습니다!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("코인이 부족합니다!", Color.red, 2.0f));
        }
    }

    public void UpgradeTowerBuildSpeed(int coinNum, int speed)
    {
        if (playerMovement.coinCount >= coinNum)
        {
            playerMovement.coinCount -= coinNum;
            laboratory.UpgradeTowerBuildSpeed(tower, speed);
            StartCoroutine(ShowActionText("타워 건설 속도가 증가되었습니다!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("코인이 부족합니다!", Color.red, 2.0f));
        }
    }

    public void UpgradeTowerHP(int coinNum, int hp)
    {
        if (playerMovement.coinCount >= coinNum)
        {
            playerMovement.coinCount -= coinNum;
            laboratory.UpgradeTowerHP(tower, hp);
            StartCoroutine(ShowActionText("타워 생명력이 강화되었습니다!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("코인이 부족합니다!", Color.red, 2.0f));
        }
    }

    public void UpgradeTowerDef(int coinNum, int def)
    {
        if (playerMovement.coinCount >= coinNum)
        {
            playerMovement.coinCount -= coinNum;
            laboratory.UpgradeTowerDef(tower, def);
            StartCoroutine(ShowActionText("타워 방어력이 강화되었습니다!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("코인이 부족합니다!", Color.red, 2.0f));
        }
    }

    public void UpgradeTowerAttackSpeed(int coinNum, int attSpeed)
    {
        if (playerMovement.coinCount >= coinNum)
        {
            playerMovement.coinCount -= coinNum;
            laboratory.UpgradeTowerAttackSpeed(tower, attSpeed);
            StartCoroutine(ShowActionText("타워 공격속도가 강화되었습니다!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("코인이 부족합니다!", Color.red, 2.0f));
        }
    }

    public void UpgradeResourceRespawnSpeed(int coinNum)
    {
        if (playerMovement.coinCount >= coinNum)
        {
            playerMovement.coinCount -= coinNum;
            laboratory.UpgradeResourceRespawnSpeed();
            StartCoroutine(ShowActionText("자원 생성 속도가 강화되었습니다!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("코인이 부족합니다!", Color.red, 2.0f));
        }
    }
}
