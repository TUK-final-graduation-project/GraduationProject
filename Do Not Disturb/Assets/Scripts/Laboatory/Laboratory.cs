using UnityEngine;
using UnityEngine.UI;

public class Laboratory : MonoBehaviour
{
    [SerializeField]
    GameObject uiPanel; // 연구소 UI 패널

    public PlayerMovement player;
    public ResourceManager manager;
    public UserHome home;
    public Tower[] towers;
    public OurUnitController[] userUnits;
    public EnemyUnitController[] enemyUnits;

    // 강화에 필요한 코인 개수
    public int PlayerSpeedCost = 50;
    public int BaseHPCost = 100;
    public int ResourceRespawnTimeCost = 50;

    public int ResourceRequiredItemsCost = 100;
    public int TowerHPCost = 100;
    public int TowerSpeedCost = 100;

    public int UserUnitHP = 100;
    public int UserUnitSpeed = 100;
    public int EnemyUnitHP = 100;
    public int EnemyUnitSpeed = 100;

    // 버튼 및 텍스트 컴포넌트
    public Button[] upgradeButton = new Button[10];
    public Text[] upgradeButtonText = new Text[10];

    private int userBaseMaxHP = 100;

    void Start()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(false); // UI 패널을 비활성화
            Cursor.visible = false; // 커서 숨기기
        }
        UpdateButtonUI();
    }

    public void UpdateButtonUI()
    {
        // player 또는 upgradeButtonText가 null인지 확인
        if (player == null || upgradeButtonText == null)
        {
            //Debug.Log("Player or upgradeButtonText is null");
            return;
        }

        for (int i = 0; i < upgradeButtonText.Length; i++)
        {
            if (upgradeButtonText[i] == null)
            {
                //Debug.Log("upgradeButtonText[" + i + "] is null");
                return;
            }
        }

        upgradeButtonText[0].text = PlayerSpeedCost + " coins";
        upgradeButtonText[1].text = BaseHPCost + " coins";
        upgradeButtonText[2].text = ResourceRespawnTimeCost + " coins";
        upgradeButtonText[3].text = ResourceRequiredItemsCost + " coins";
        upgradeButtonText[4].text = TowerHPCost + " coins";
        upgradeButtonText[5].text = TowerSpeedCost + " coins";
        upgradeButtonText[6].text = UserUnitHP + " coins";
        upgradeButtonText[7].text = UserUnitSpeed + " coins";
        upgradeButtonText[8].text = EnemyUnitHP + " coins";
        upgradeButtonText[9].text = EnemyUnitSpeed + " coins";

        // 코인 확인 후 색상 변경
        UpdateButtonColor(upgradeButton[0], player.coinCount >= PlayerSpeedCost);
        UpdateButtonColor(upgradeButton[1], player.coinCount >= BaseHPCost);
        UpdateButtonColor(upgradeButton[2], player.coinCount >= ResourceRespawnTimeCost);
        UpdateButtonColor(upgradeButton[3], player.coinCount >= ResourceRequiredItemsCost);
        UpdateButtonColor(upgradeButton[4], player.coinCount >= TowerHPCost);
        UpdateButtonColor(upgradeButton[5], player.coinCount >= TowerSpeedCost);
        UpdateButtonColor(upgradeButton[6], player.coinCount >= UserUnitHP);
        UpdateButtonColor(upgradeButton[7], player.coinCount >= UserUnitSpeed);
        UpdateButtonColor(upgradeButton[8], player.coinCount >= EnemyUnitHP);
        UpdateButtonColor(upgradeButton[9], player.coinCount >= EnemyUnitSpeed);
    }

    public void UpdateButtonColor(Button button, bool isAffordable)
    {
        if (button == null)
        {
            Debug.LogError("Button is null");
            return;
        }

        ColorBlock colors = button.colors;
        if (isAffordable)
        {
            colors.normalColor = Color.white;
            colors.selectedColor = Color.white;
            colors.highlightedColor = Color.green;
        }
        else
        {
            colors.normalColor = Color.red;
            colors.selectedColor = Color.red;
            colors.highlightedColor = Color.red;
        }
        button.colors = colors;
    }

    void Update()
    {
        if (upgradeButtonText == null || player == null)
        {
            return;
        }

        UpdateButtonUI();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (uiPanel != null)
            {
                uiPanel.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    public void UpgradePlayerSpeed()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Level);
        player.SetSpeed(player.speed + 5);
        player.SetRunSpeed(player.runSpeed + 5);
        Debug.Log("coinCount : " + player.coinCount + "| speed : " + player.speed);
    }

    public void UpgradePlayerHP()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Level);
        player.SetHP(player.maxHP);
        Debug.Log("coinCount : " + player.coinCount + "| HP : " + player.HP);
    }

    public void UpgradePlayerDamage()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Level);
        player.SetDamage(player.attackDamage + 20);
        Debug.Log("coinCount : " + player.coinCount + "| attackDamage : " + player.attackDamage);
    }

    public void UpgradeUserBase()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Level);
        home.setHP(userBaseMaxHP);
        Debug.Log("coinCount : " + player.coinCount + "| userBaseHP : " + home.baseHP);
    }

    public void UpgradeResourceRespawnSpeed()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Level);
        manager.MinusRespawnTime(10);
        Debug.Log("coinCount : " + player.coinCount + "| respawnTime : " + manager.respawnTime);
    }

    public void UpgradeTowerHP()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Level);
        towers = FindObjectsOfType(typeof(Tower)) as Tower[];
        foreach (Tower tower in towers)
        {
            tower.SetHP(tower.HP + 50);
        }
    }

    public void UpgradeTowerAttackSpeed()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Level);
        towers = FindObjectsOfType(typeof(Tower)) as Tower[];
        foreach (Tower tower in towers)
        {
            tower.SetAttackSpeed(tower.attackSpeed);
        }
    }

    //------------------------------
    public void UpgradeUserUnitSpeed()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Level);
        userUnits = FindObjectsOfType(typeof(OurUnitController)) as OurUnitController[];
        foreach (OurUnitController userUnit in userUnits)
        {
            userUnit.SetSpeed(userUnit.speed + 5);
        }
    }
    public void UpgradeEnemyUnitSpeed()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Level);
        enemyUnits = FindObjectsOfType(typeof(EnemyUnitController)) as EnemyUnitController[];
        foreach (EnemyUnitController enemyUnit in enemyUnits)
        {
            enemyUnit.SetSpeed(enemyUnit.speed / 2);
        }
    }

    public void UpgradeUserUnitHP()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Level);
        //userUnits.SetHP(userUnits.HP + 50);
    }
    public void UpgradeEnemyUnitHP()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Level);
        //enemyUnit.SetHP(enemyUnit.HP - 30);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (uiPanel != null)
            {
                uiPanel.SetActive(true); // 플레이어가 연구소 근처에 왔을 때 UI 패널을 활성화
                Cursor.visible = true; // 커서 보이기
                Cursor.lockState = CursorLockMode.None;
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Lab);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (uiPanel != null)
            {
                uiPanel.SetActive(false); // 플레이어가 연구소에서 벗어났을 때 UI 패널을 비활성화
                Cursor.visible = false; // 커서 숨기기
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
}
