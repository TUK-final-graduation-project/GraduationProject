using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Laboratory : MonoBehaviour
{
    [SerializeField]
    GameObject uiPanel;

    private PlayerMovement player;
    public ResourceManager manager;
    public UserHome home;
    public Tower tower;

    public int PlayerSpeedCost = 50;
    public int BaseHPCost = 100;
    public int ResourceRespawnTimeCost = 50;
    public int ResourceRequiredItemsCost = 100;
    public int TowerHPCost = 100;
    public int TowerSpeedCost = 100;
    public int UserUnitHPCost = 100;
    public int UserUnitSpeedCost = 100;
    public int EnemyUnitHPCost = 100;
    public int EnemyUnitSpeedCost = 100;

    public Button[] upgradeButton = new Button[10];
    public Text[] upgradeButtonText = new Text[10];

    private int userBaseMaxHP = 100;

    void Start()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
            Cursor.visible = false;
        }


        UpdateButtonUI();
    }

    public void SetPlayer(PlayerMovement player)
    {
        this.player = player;
        UpdateButtonUI();
    }

    public void UpdateButtonUI()
    {
        if (player == null) return;

        upgradeButtonText[0].text = PlayerSpeedCost + " coins";
        upgradeButtonText[1].text = BaseHPCost + " coins";
        upgradeButtonText[2].text = ResourceRespawnTimeCost + " coins";
        upgradeButtonText[3].text = ResourceRequiredItemsCost + " coins";
        upgradeButtonText[4].text = TowerHPCost + " coins";
        upgradeButtonText[5].text = TowerSpeedCost + " coins";
        upgradeButtonText[6].text = UserUnitHPCost + " coins";
        upgradeButtonText[7].text = UserUnitSpeedCost + " coins";
        upgradeButtonText[8].text = EnemyUnitHPCost + " coins";
        upgradeButtonText[9].text = EnemyUnitSpeedCost + " coins";

        UpdateButtonColor(upgradeButton[0], player.coinCount >= PlayerSpeedCost);
        UpdateButtonColor(upgradeButton[1], player.coinCount >= BaseHPCost);
        UpdateButtonColor(upgradeButton[2], player.coinCount >= ResourceRespawnTimeCost);
        UpdateButtonColor(upgradeButton[3], player.coinCount >= ResourceRequiredItemsCost);
        UpdateButtonColor(upgradeButton[4], player.coinCount >= TowerHPCost);
        UpdateButtonColor(upgradeButton[5], player.coinCount >= TowerSpeedCost);
        UpdateButtonColor(upgradeButton[6], player.coinCount >= UserUnitHPCost);
        UpdateButtonColor(upgradeButton[7], player.coinCount >= UserUnitSpeedCost);
        UpdateButtonColor(upgradeButton[8], player.coinCount >= EnemyUnitHPCost);
        UpdateButtonColor(upgradeButton[9], player.coinCount >= EnemyUnitSpeedCost);
    }

    public void UpdateButtonColor(Button button, bool isAffordable)
    {
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
        if (player != null)
        {
            UpdateButtonUI();
        }
    }

    public void UpgradePlayerSpeed()
    {
        if (player == null) return;

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Level);
        player.pv.RPC("RPC_UpgradePlayerSpeed", RpcTarget.All, player.speed + 5, player.runSpeed + 5);
        Debug.Log("coinCount : " + player.coinCount + "| speed : " + player.speed);
    }

    public void UpgradeUserBase()
    {
        if (player == null) return;

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Level);
        //manager.GetUserHome().UpgradeMaxHP();
    }

    public void UpgradeResourceRespawnSpeed()
    {
        if (player == null) return;

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Level);
        //manager.UpgradeRespawnSpeed();
    }

    public void UpgradeTowerHP()
    {
        if (player == null) return;

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Level);
        //manager.GetTower().UpgradeMaxHP();
    }

    public void UpgradeTowerSpeed()
    {
        if (player == null) return;

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Level);
        //manager.GetTower().UpgradeAttackSpeed();
    }

    public void UpgradeUserUnitHP()
    {
        if (player == null) return;

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Level);
        //manager.UpgradeUserUnitMaxHP();
    }

    public void UpgradeUserUnitSpeed()
    {
        if (player == null) return;

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Level);
        //manager.UpgradeUserUnitAttackSpeed();
    }

    public void UpgradeEnemyUnitHP()
    {
        if (player == null) return;

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Level);
        //manager.UpgradeEnemyUnitMaxHP();
    }

    public void UpgradeEnemyUnitSpeed()
    {
        if (player == null) return;

        AudioManager.instance.PlaySfx(AudioManager.Sfx.Level);
       // manager.UpgradeEnemyUnitAttackSpeed();
    }

    public void Show()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(true);
            Cursor.visible = true;
        }
    }

    public void Hide()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
            Cursor.visible = false;
        }
    }
}
