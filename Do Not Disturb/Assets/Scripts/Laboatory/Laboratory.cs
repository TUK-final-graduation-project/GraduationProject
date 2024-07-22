using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laboratory : MonoBehaviour
{
    [SerializeField]
    GameObject uiPanel; // 연구소 UI 패널

    public PlayerMovement player;
    public ResourceManager manager;
    public UserHome home;

    private int userBaseMaxHP = 100;

    void Start()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(false); // UI 패널을 비활성화
            Cursor.visible = false; // 커서 숨기기
        }
    }

    void Update()
    {
    }

    public void UpgradePlayerSpeed()
    {
        player.SetSpeed(player.speed + 20);
        Debug.Log("coinCount : " + player.coinCount + "| speed : " + player.speed);
    }

    public void UpgradePlayerHP()
    {
        player.SetHP(player.maxHP);
        Debug.Log("coinCount : " + player.coinCount + "| HP : " + player.HP);
    }

    public void UpgradePlayerDamage()
    {
        player.SetDamage(player.attackDamage + 20);
        Debug.Log("coinCount : " + player.coinCount + "| attackDamage : " + player.attackDamage);
    }

    public void UpgradeUserBase()
    {
        home.setHP(userBaseMaxHP);
    }

    public void UpgradeTowerHP(Tower tower, int newHP)
    {
        tower.SetHP(newHP);
    }

    public void UpgradeTowerAttackSpeed(Tower tower, float speed)
    {
        tower.SetAttackSpeed(speed);
    }

    public void UpgradeTowerDef(Tower tower, int def)
    {
        tower.SetDef(def);
    }

    public void UpgradeTowerBuildSpeed(Tower tower, float buildSpeed)
    {
        // Implement this if needed
    }

    public void UpgradeResourceRespawnSpeed()
    {
        manager.MinusRespawnTime(10);
        Debug.Log("coinCount : " + player.coinCount + "| respawnTime : " + manager.respawnTime);
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
