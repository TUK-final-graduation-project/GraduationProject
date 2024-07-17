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

    void Start()
    {
        itemCount[0] = inventorySlot_rock.itemCount;
        itemCount[1] = inventorySlot_steel.itemCount;
        itemCount[2] = inventorySlot_wood.itemCount;
        itemCount[3] = inventorySlot_ice.itemCount;

        if (uiPanel != null)
        {
            uiPanel.SetActive(false); // UI �г��� ��Ȱ��ȭ
        }
    }

    void Update()
    {
        itemCount[0] = inventorySlot_rock.itemCount;
        itemCount[1] = inventorySlot_steel.itemCount;
        itemCount[2] = inventorySlot_wood.itemCount;
        itemCount[3] = inventorySlot_ice.itemCount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (uiPanel != null)
            {
                uiPanel.SetActive(true); // �÷��̾ ������ ��ó�� ���� �� UI �г��� Ȱ��ȭ
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
            }
        }
    }
}
