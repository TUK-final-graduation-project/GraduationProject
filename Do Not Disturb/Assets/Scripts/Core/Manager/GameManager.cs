using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
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
            //Debug.LogError("PlayerMovement ������Ʈ�� ã�� �� �����ϴ�.");
        }
        
        laboratory = FindObjectOfType<Laboratory>();
        if (laboratory == null)
        {
            //Debug.LogError("laboratory ������Ʈ�� ã�� �� �����ϴ�.");
        }

        StartCoroutine(UpdateCoins()); // Start the coroutine to update coins

        bossHealthGroup.transform.gameObject.SetActive(false);
        Invoke("SetBgm", 1);
    }

    void SetBgm()
    {
        AudioManager.instance.WPlayBgm(true);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            // isPaused�� true�̸� �Ͻ� ���� ����
            if (isPaused)
            {
                ResumeSetting();
            }
            // isPaused�� false�̸� �Ͻ� ����
            else
            {
                PauseSetting();
            }
        }
        /*if (state == States.ready)
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
        }*/
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
            AudioManager.instance.WPlayBgm(false);
            if (stage == 4)
            {
                AudioManager.instance.SPlayBgm(true);
                Bosses[0].SetActive(true);
                bossHealthGroup.gameObject.SetActive(true);
            }
            else if (stage == 7)
            {
                AudioManager.instance.SPlayBgm(true);
                Bosses[1].SetActive(true);
                bossHealthGroup.gameObject.SetActive(true);
            }
            else if (stage == 10)
            {
                AudioManager.instance.SPlayBgm(true);
                Bosses[2].SetActive(true);
                bossHealthGroup.gameObject.SetActive(true);
            }
            else
            {
                AudioManager.instance.BPlayBgm(true);
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

            AudioManager.instance.SPlayBgm(false);
            AudioManager.instance.BPlayBgm(false);
            AudioManager.instance.WPlayBgm(true);
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
        foreach (ComSpawnPoint spawn in spawns)
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

    void fail()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Fail);
        StartCoroutine(ShowActionText("������ �����մϴ�!", Color.red, 2.0f));
    }

    public void UpgradePlayerSpeed()
    {
        if (playerMovement.coinCount >= laboratory.PlayerSpeedCost)
        {
            playerMovement.coinCount -= laboratory.PlayerSpeedCost;
            laboratory.UpgradePlayerSpeed();
            photonView.RPC("RPC_UpgradePlayerSpeed", RpcTarget.All);
        }
        else
        {
            fail();
        }
    }

    public void UpgradeBase()
    {
        if (playerMovement.coinCount >= laboratory.BaseHPCost)
        {
            playerMovement.coinCount -= laboratory.BaseHPCost;
            laboratory.UpgradeUserBase();
            photonView.RPC("RPC_UpgradeBase", RpcTarget.All);
        }
        else
        {
            fail();
        }
    }

    public void UpgradeResourceRespawnSpeed()
    {
        if (playerMovement.coinCount >= laboratory.ResourceRespawnTimeCost)
        {
            playerMovement.coinCount -= laboratory.ResourceRespawnTimeCost;
            laboratory.UpgradeResourceRespawnSpeed();
            photonView.RPC("RPC_UpgradeResourceRespawnSpeed", RpcTarget.All);
        }
        else
        {
            fail();
        }
    }

    public void UpgradeDiscountItem()
    {
        if (craftMenu != null)
        {
            if (playerMovement.coinCount >= laboratory.ResourceRequiredItemsCost)
            {
                playerMovement.coinCount -= laboratory.ResourceRequiredItemsCost;
                craftMenu.ApplyDiscount(0.5f); // �Ű������� ������ (/100)
                StartCoroutine(ShowActionText("�Ǽ� �Ҹ� �������� ���� �Ǿ����ϴ�!", Color.green, 2.0f));
                photonView.RPC("RPC_UpgradeDiscountItem", RpcTarget.All, 0.5f);
            }
            else
            {
                fail();
            }
        }
        else
        {
            Debug.LogError("CraftMenu �ν��Ͻ��� �����ϴ�.");
        }
    }

    public void UpgradeTowerHP()
    {
        if (playerMovement.coinCount >= laboratory.TowerHPCost)
        {
            playerMovement.coinCount -= laboratory.TowerHPCost;
            laboratory.UpgradeTowerHP();
            photonView.RPC("RPC_UpgradeTowerHP", RpcTarget.All);
        }
        else
        {
            fail();
        }
    }

    public void UpgradeTowerSpeed()
    {
        if (playerMovement.coinCount >= laboratory.TowerSpeedCost)
        {
            playerMovement.coinCount -= laboratory.TowerSpeedCost;
            laboratory.UpgradeTowerSpeed();
            photonView.RPC("RPC_UpgradeTowerSpeed", RpcTarget.All);
        }
        else
        {
            fail();
        }
    }

    public void UpgradeUserUnitHP()
    {
        if (playerMovement.coinCount >= laboratory.UserUnitHPCost)
        {
            playerMovement.coinCount -= laboratory.UserUnitHPCost;
            laboratory.UpgradeUserUnitHP();
            photonView.RPC("RPC_UpgradeUserUnitHP", RpcTarget.All);
        }
        else
        {
            fail();
        }
    }

    public void UpgradeUserUnitSpeed()
    {
        if (playerMovement.coinCount >= laboratory.UserUnitSpeedCost)
        {
            playerMovement.coinCount -= laboratory.UserUnitSpeedCost;
            laboratory.UpgradeUserUnitSpeed();
            photonView.RPC("RPC_UpgradeUserUnitSpeed", RpcTarget.All);
        }
        else
        {
            fail();
        }
    }

    public void UpgradeEnemyUnitHP()
    {
        if (playerMovement.coinCount >= laboratory.EnemyUnitHPCost)
        {
            playerMovement.coinCount -= laboratory.EnemyUnitHPCost;
            laboratory.UpgradeEnemyUnitHP();
            photonView.RPC("RPC_UpgradeEnemyUnitHP", RpcTarget.All);
        }
        else
        {
            fail();
        }
    }

    public void UpgradeEnemyUnitSpeed()
    {
        if (playerMovement.coinCount >= laboratory.EnemyUnitSpeedCost)
        {
            playerMovement.coinCount -= laboratory.EnemyUnitSpeedCost;
            laboratory.UpgradeEnemyUnitSpeed();
            photonView.RPC("RPC_UpgradeEnemyUnitSpeed", RpcTarget.All);
        }
        else
        {
            fail();
        }
    }

    [PunRPC]
    public void RPC_UpgradePlayerSpeed()
    {
        if (playerMovement != null)
        {
            playerMovement.UpgradePlayerSpeed(playerMovement.speed + 5, playerMovement.runSpeed + 5);
            StartCoroutine(ShowActionText("�̵��ӵ��� ���� �Ǿ����ϴ�!", Color.green, 2.0f));
        }
    }

    [PunRPC]
    public void RPC_UpgradeBase()
    {
        if (laboratory != null)
        {
            laboratory.UpgradeUserBase();
            StartCoroutine(ShowActionText("������ �ִ� ü���� ���� �Ǿ����ϴ�!", Color.green, 2.0f));
        }
    }

    [PunRPC]
    public void RPC_UpgradeResourceRespawnSpeed()
    {
        if (laboratory != null)
        {
            laboratory.UpgradeResourceRespawnSpeed();
            StartCoroutine(ShowActionText("�ڿ� ���� �ӵ��� ���� �Ǿ����ϴ�!", Color.green, 2.0f));
        }
    }

    [PunRPC]
    public void RPC_UpgradeDiscountItem(float discountRate)
    {
        if (craftMenu != null)
        {
            craftMenu.ApplyDiscount(discountRate);
            StartCoroutine(ShowActionText("�Ǽ� �Ҹ� �������� ���� �Ǿ����ϴ�!", Color.green, 2.0f));
        }
    }

    [PunRPC]
    public void RPC_UpgradeTowerHP()
    {
        if (laboratory != null)
        {
            laboratory.UpgradeTowerHP();
            StartCoroutine(ShowActionText("Ÿ���� �ִ� ü���� ���� �Ǿ����ϴ�!", Color.green, 2.0f));
        }
    }

    [PunRPC]
    public void RPC_UpgradeTowerSpeed()
    {
        if (laboratory != null)
        {
            laboratory.UpgradeTowerSpeed();
            StartCoroutine(ShowActionText("Ÿ���� ���ݼӵ��� ���� �Ǿ����ϴ�!", Color.green, 2.0f));
        }
    }

    [PunRPC]
    public void RPC_UpgradeUserUnitHP()
    {
        if (laboratory != null)
        {
            laboratory.UpgradeUserUnitHP();
            StartCoroutine(ShowActionText("����� ������ �ִ� ü���� ���� �Ǿ����ϴ�!", Color.green, 2.0f));
        }
    }

    [PunRPC]
    public void RPC_UpgradeUserUnitSpeed()
    {
        if (laboratory != null)
        {
            laboratory.UpgradeUserUnitSpeed();
            StartCoroutine(ShowActionText("����� ������ ���ݼӵ��� ���� �Ǿ����ϴ�!", Color.green, 2.0f));
        }
    }

    [PunRPC]
    public void RPC_UpgradeEnemyUnitHP()
    {
        if (laboratory != null)
        {
            laboratory.UpgradeEnemyUnitHP();
            StartCoroutine(ShowActionText("�� ������ �ִ� ü���� ���� �Ǿ����ϴ�!", Color.green, 2.0f));
        }
    }

    [PunRPC]
    public void RPC_UpgradeEnemyUnitSpeed()
    {
        if (laboratory != null)
        {
            laboratory.UpgradeEnemyUnitSpeed();
            StartCoroutine(ShowActionText("�� ������ ���ݼӵ��� ���� �Ǿ����ϴ�!", Color.green, 2.0f));
        }
    }

    public void SetPlayerMovement(PlayerMovement playerMovement)
    {
        this.playerMovement = playerMovement;
        laboratory.SetPlayer(playerMovement);
    }
}
