using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laboratory : MonoBehaviour
{
    public int[] itemCount = new int[4];

    [SerializeField]
    InventorySlot inventorySlot_rock;

    [SerializeField]
    InventorySlot inventorySlot_steel;

    [SerializeField]
    InventorySlot inventorySlot_wood;

    [SerializeField]
    InventorySlot inventorySlot_ice;

    [SerializeField]
    GameObject uiPanel; // ������ UI �г�

    public PlayerMovement player;

    void Start()
    {
        itemCount[0] = inventorySlot_rock.itemCount;
        itemCount[1] = inventorySlot_steel.itemCount;
        itemCount[2] = inventorySlot_wood.itemCount;
        itemCount[3] = inventorySlot_ice.itemCount;

        if (uiPanel != null)
        {
            uiPanel.SetActive(false); // UI �г��� ��Ȱ��ȭ
            Cursor.visible = false; // Ŀ�� �����
        }
    }

    void Update()
    {
        itemCount[0] = inventorySlot_rock.itemCount;
        itemCount[1] = inventorySlot_steel.itemCount;
        itemCount[2] = inventorySlot_wood.itemCount;
        itemCount[3] = inventorySlot_ice.itemCount;
    }

    public void UpgradePlayerSpeed()
    {
        // �ʿ� ���� ���� 20
        if (player.coinCount >= 20)
        {
            player.coinCount -= 20;
            player.SetSpeed(player.speed + 1);
            Debug.Log("coinCount : " + player.coinCount + "| speed : " + player.speed);
        }
    }

    public void UpgradePlayerHP()
    {
        if (player.coinCount >= 20)
        {
            player.coinCount -= 20;
            player.SetHP(player.maxHP);
            Debug.Log("coinCount : " + player.coinCount + "| HP : " + player.HP);
        }
    }

    public void UpgradePlayerDamage()
    {
        if (player.coinCount >= 20)
        {
            player.coinCount -= 20;
            player.SetDamage(player.attackDamage + 20);
            Debug.Log("coinCount : " + player.coinCount + "| attackDamage : " + player.attackDamage);
        }
    }

    public void UpgradeUserBase()
    {
        
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
