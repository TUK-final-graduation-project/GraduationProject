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

    public GameObject settingsPanel; // ���� UI �г�
    public GameObject scorePanel; // ���� UI �г�

    private CraftMenu craftMenu; // CraftMenu Ŭ���� �ν��Ͻ� ���� �߰�
    private PlayerMovement playerMovement; // Reference to PlayerMovement

    float uiTime;

    private void Awake()
    {
        state = States.ready;
        uiTime = readyTime;
        readyTime = coolTimeOfReady;
        battleTime = coolTimeOfBattle;
        isPaused = false;

        // CraftMenu �ν��Ͻ� �ʱ�ȭ
        craftMenu = GetComponent<CraftMenu>();
        if (craftMenu == null)
        {
            Debug.LogError("CraftMenu ������Ʈ�� ã�� �� �����ϴ�.");
        }

        // Find the PlayerMovement component in the scene
        playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement ������Ʈ�� ã�� �� �����ϴ�.");
        }

        StartCoroutine(UpdateCoins()); // Start the coroutine to update coins
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            // isPuase�� true�̸� �Ͻ� ���� ����
            if (isPaused)
            {
                ResumeSetting();
            }
            // isPuase�� false�̸� �Ͻ� ����
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
        Time.timeScale = 0; // ���� �Ͻ� ����
        isPaused = true; // ���� ����
        settingsPanel.SetActive(true); // ���� UI ǥ��
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ResumeSetting()
    {
        Time.timeScale = 1; // ���� �簳
        isPaused = false; // ���� ����
        settingsPanel.SetActive(false); // ���� UI ����
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
                craftMenu.ApplyDiscount(coinNum); // ApplyDiscount�� CraftMenu Ŭ������ ������ �޼���
                StartCoroutine(ShowActionText("�Ǽ� �������� ���� �Ǿ����ϴ�!", Color.green, 2.0f));
            }
            else
            {
                StartCoroutine(ShowActionText("������ �����մϴ�!", Color.red, 2.0f));
            }
        }
        else
        {
            Debug.LogError("CraftMenu �ν��Ͻ��� �����ϴ�.");
        }
    }

    public void UpgradePlayerSpeed(int coinNum)
    {
        if (playerMovement.coinCount >= coinNum)
        {
            playerMovement.coinCount -= coinNum;
            laboratory.UpgradePlayerSpeed();
            StartCoroutine(ShowActionText("�̵��ӵ��� ���� �Ǿ����ϴ�!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("������ �����մϴ�!", Color.red, 2.0f));
        }
    }

    public void UpgradePlayerHP(int coinNum)
    {
        if (playerMovement.coinCount >= coinNum)
        {
            playerMovement.coinCount -= coinNum;
            laboratory.UpgradePlayerHP();
            StartCoroutine(ShowActionText("�÷��̾��� ü���� ��ȭ�Ǿ����ϴ�!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("������ �����մϴ�!", Color.red, 2.0f));
        }
    }

    public void UpgradePlayerDamage(int coinNum)
    {
        if (playerMovement.coinCount >= coinNum)
        {
            playerMovement.coinCount -= coinNum;
            laboratory.UpgradePlayerDamage();
            StartCoroutine(ShowActionText("�÷��̾� ���ݷ��� ��ȭ�Ǿ����ϴ�!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("������ �����մϴ�!", Color.red, 2.0f));
        }
    }

    public void UpgradeBase(int coinNum)
    {
        if (playerMovement.coinCount >= coinNum)
        {
            playerMovement.coinCount -= coinNum;
            laboratory.UpgradeUserBase();
            StartCoroutine(ShowActionText("���� ������� ȸ���Ǿ����ϴ�!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("������ �����մϴ�!", Color.red, 2.0f));
        }
    }

    public void UpgradeTowerBuildSpeed(int coinNum, int speed)
    {
        if (playerMovement.coinCount >= coinNum)
        {
            playerMovement.coinCount -= coinNum;
            laboratory.UpgradeTowerBuildSpeed(tower, speed);
            StartCoroutine(ShowActionText("Ÿ�� �Ǽ� �ӵ��� �����Ǿ����ϴ�!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("������ �����մϴ�!", Color.red, 2.0f));
        }
    }

    public void UpgradeTowerHP(int coinNum, int hp)
    {
        if (playerMovement.coinCount >= coinNum)
        {
            playerMovement.coinCount -= coinNum;
            laboratory.UpgradeTowerHP(tower, hp);
            StartCoroutine(ShowActionText("Ÿ�� ������� ��ȭ�Ǿ����ϴ�!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("������ �����մϴ�!", Color.red, 2.0f));
        }
    }

    public void UpgradeTowerDef(int coinNum, int def)
    {
        if (playerMovement.coinCount >= coinNum)
        {
            playerMovement.coinCount -= coinNum;
            laboratory.UpgradeTowerDef(tower, def);
            StartCoroutine(ShowActionText("Ÿ�� ������ ��ȭ�Ǿ����ϴ�!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("������ �����մϴ�!", Color.red, 2.0f));
        }
    }

    public void UpgradeTowerAttackSpeed(int coinNum, int attSpeed)
    {
        if (playerMovement.coinCount >= coinNum)
        {
            playerMovement.coinCount -= coinNum;
            laboratory.UpgradeTowerAttackSpeed(tower, attSpeed);
            StartCoroutine(ShowActionText("Ÿ�� ���ݼӵ��� ��ȭ�Ǿ����ϴ�!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("������ �����մϴ�!", Color.red, 2.0f));
        }
    }

    public void UpgradeResourceRespawnSpeed(int coinNum)
    {
        if (playerMovement.coinCount >= coinNum)
        {
            playerMovement.coinCount -= coinNum;
            laboratory.UpgradeResourceRespawnSpeed();
            StartCoroutine(ShowActionText("�ڿ� ���� �ӵ��� ��ȭ�Ǿ����ϴ�!", Color.green, 2.0f));
        }
        else
        {
            StartCoroutine(ShowActionText("������ �����մϴ�!", Color.red, 2.0f));
        }
    }
}
