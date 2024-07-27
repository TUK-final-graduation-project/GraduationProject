using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Video;

public class GameManager : MonoBehaviour
{
    public enum States { ready, battle, bossVideo, gameEnd };

    public int stage;
    public int maxStage;
    public States state;
    public int enemyCnt;

    public float battleTime;
    public float readyTime;
    public float bossVideoTime;
    public float playTime;

    public float coolTimeOfReady;
    public float coolTimeOfBattle;
    public float coolTimeOfbossVideo;

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
    [SerializeField]
    public VideoPlayer Boss_Rock;
    [SerializeField]
    public VideoPlayer Boss_Orc;
    [SerializeField]
    public VideoPlayer Boss_Wizard;

    [SerializeField]
    public GameObject videoCanvas;

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

        Invoke("SetBgm", 1);
    }

    private void Update()
    {
        playTime += Time.deltaTime;

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

        switch (state)
        {
            case States.ready:
                {
                    Ready();
                    break;
                }
            case States.battle:
                {
                    Battle();
                    break;
                }
            case States.bossVideo:
                {
                    BossVideo();
                    break;
                }
        }

        if (CalculateGameEnd())
        {
            SceneManager.LoadScene(2);
        }
    }

    void SetBgm()
    {
        AudioManager.instance.WPlayBgm(true);
    }

    void Ready()
    {
        // �ð� ������Ʈ
        readyTime -= Time.deltaTime;
        uiTime = readyTime;

        if (readyTime < 0)
        {
            AudioManager.instance.WPlayBgm(false);
            state = States.bossVideo;
            readyTime = coolTimeOfReady;

            if (stage == 4)
            {
                PlayVideoOnTime(9f,0);
                bossVideoTime = 10f;
            }
            else if (stage == 7)
            {
                PlayVideoOnTime(7f,1);
                bossVideoTime = 8f;
            }
            else if (stage == 10)
            {
                PlayVideoOnTime(5f, 2);
                bossVideoTime = 6f;
            }
            else
            {
                state = States.battle;
                AudioManager.instance.BPlayBgm(true);
                foreach (ComSpawnPoint spawn in spawns)
                {
                    spawn.StartSpawn(stage + 1);
                }
            }
        }
    }

    void Battle()
    {
        // �ð� ������Ʈ
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

            // ���̺갡 ���� ���
            if (stage == (maxStage + 1))
            {
                stage = maxStage;
                state = States.gameEnd;

                SceneManager.LoadScene(2/* �¸� �� */);
            }

        }
    }

    void BossVideo()
    {
        bossVideoTime -= Time.deltaTime;
        
        if (bossVideoTime < 0)
        {
            state = States.battle;

            AudioManager.instance.WPlayBgm(false);
            state = States.battle;
            readyTime = coolTimeOfReady;

            AudioManager.instance.SPlayBgm(true);
            bossHealthGroup.gameObject.SetActive(true);
            if (stage == 4)
            {
                Bosses[0].SetActive(true);
            }
            else if (stage == 7)
            {
                Bosses[1].SetActive(true);
            }
            else if (stage == 10)
            {
                Bosses[2].SetActive(true);
            }
            foreach (ComSpawnPoint spawn in spawns)
            {
                spawn.StartSpawn(stage + 1);
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
            StartCoroutine(ShowActionText("�̵��ӵ��� ���� �Ǿ����ϴ�!", Color.green, 2.0f));
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
            StartCoroutine(ShowActionText("���� ������� ȸ���Ǿ����ϴ�!", Color.green, 2.0f));
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
            StartCoroutine(ShowActionText("�ڿ� ���� �ӵ��� ���������ϴ�!", Color.green, 2.0f));
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
            StartCoroutine(ShowActionText("Ÿ�� ������� ��ȭ�Ǿ����ϴ�!", Color.green, 2.0f));
        }
        else
        {
            fail();
        }
    }

    public void UpgradeTowerAttackSpeed()
    {
        if (playerMovement.coinCount >= laboratory.TowerSpeedCost)
        {
            playerMovement.coinCount -= laboratory.TowerSpeedCost;
            laboratory.UpgradeTowerAttackSpeed();
            StartCoroutine(ShowActionText("Ÿ�� ���ݼӵ��� �������ϴ�!", Color.green, 2.0f));
        }
        else
        {
            fail();
        }
    }

    public void UpgradeUserUnitSpeed()
    {
        if (playerMovement.coinCount >= laboratory.UserUnitHP)
        {
            playerMovement.coinCount -= laboratory.UserUnitHP;
            laboratory.UpgradeUserUnitSpeed();
            StartCoroutine(ShowActionText("�Ʊ� ���� �̵��ӵ��� �������ϴ�!", Color.green, 2.0f));
        }
        else
        {
            fail();
        }
    }

    public void UpgradeUserUnitDamage()
    {
        if (playerMovement.coinCount >= laboratory.UserUnitSpeed)
        {
            playerMovement.coinCount -= laboratory.UserUnitSpeed;
            laboratory.UpgradeUserUnitDamage();
            StartCoroutine(ShowActionText("�Ʊ� ���� ���ݷ��� �������ϴ�!", Color.green, 2.0f));
        }
        else
        {
            fail();
        }
    }
    public void UpgradeEnemyUnitSpeed()
    {
        if (playerMovement.coinCount >= laboratory.EnemyUnitHP)
        {
            playerMovement.coinCount -= laboratory.EnemyUnitHP;
            laboratory.UpgradeEnemyUnitSpeed();
            StartCoroutine(ShowActionText("���� ���� �̵��ӵ��� �������ϴ�!", Color.green, 2.0f));
        }
        else
        {
            fail();
        }
    }

    public void UpgradeEnemyUnitDamage()
    {
        if (playerMovement.coinCount >= laboratory.EnemyUnitSpeed)
        {
            playerMovement.coinCount -= laboratory.EnemyUnitSpeed;
            laboratory.UpgradeEnemyUnitDamage();
            StartCoroutine(ShowActionText("���� ���� ���ݷ��� �پ��ϴ�!", Color.green, 2.0f));
        }
        else
        {
            fail();
        }
    }

    public void UpdateBossHP(float bossHP, float maxHP)
    {
        bossHealth = bossHP / maxHP;
        bossHealthBar.transform.localScale = new Vector3(bossHealth, 1, 1);

        if ( bossHealth <= 0 )
        {
            bossHealthGroup.gameObject.SetActive(false);
        }
    }

    void PlayVideoOnTime(float time,int type)
    {
        isPaused = false;
        videoCanvas.gameObject.SetActive(true);
        if(type == 0)
        {
            Boss_Rock.gameObject.SetActive(true);
        }
        else if(type ==1)
        {
            Boss_Orc.gameObject.SetActive(true);
        }
        else if(type == 2)
        {
            Boss_Wizard.gameObject.SetActive(true);
        }
        StartCoroutine(WaitOffVideo(time,type));
    }

    IEnumerator WaitOffVideo(float time,int type)
    {
        yield return new WaitForSeconds(time);
        if (type == 0)
        {
            Boss_Rock.gameObject.SetActive(false);
        }
        else if (type == 1)
        {
            Boss_Orc.gameObject.SetActive(false);
        }
        else if (type == 2)
        {
            Boss_Wizard.gameObject.SetActive(false);
        }
        videoCanvas.gameObject.SetActive(false);

        isPaused = true;
     }
}
