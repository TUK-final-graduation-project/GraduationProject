using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laboratory : MonoBehaviour
{
    [SerializeField]
    GameObject uiPanel; // ������ UI �г�

    public PlayerMovement player;
    public ResourceManager manager;
    public UserHome home;

    private int userBaseMaxHP = 100;

    void Start()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(false); // UI �г��� ��Ȱ��ȭ
            Cursor.visible = false; // Ŀ�� �����
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
                uiPanel.SetActive(true); // �÷��̾ ������ ��ó�� ���� �� UI �г��� Ȱ��ȭ
                Cursor.visible = true; // Ŀ�� ���̱�
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
                uiPanel.SetActive(false); // �÷��̾ �����ҿ��� ����� �� UI �г��� ��Ȱ��ȭ
                Cursor.visible = false; // Ŀ�� �����
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
}
