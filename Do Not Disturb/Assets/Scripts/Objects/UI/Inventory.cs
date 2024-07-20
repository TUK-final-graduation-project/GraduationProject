using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public static bool inventoryActivated = false;


    // �ʿ��� ������Ʈ
    [SerializeField]
    private GameObject go_InventoryBase;
    [SerializeField]
    private GameObject go_SlotsParent;

    // ���Ե�.
    private InventorySlot[] slots;


    // Use this for initialization
    void Start()
    {
        slots = go_SlotsParent.GetComponentsInChildren<InventorySlot>();
    }

    // Update is called once per frame
    void Update()
    {
        TryOpenInventory();
    }

    private void TryOpenInventory()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryActivated = !inventoryActivated;

            if (inventoryActivated)
                OpenInventory();
            else
                CloseInventory();
        }
    }

    private void OpenInventory()
    {
        go_InventoryBase.SetActive(true);
    }

    private void CloseInventory()
    {
        go_InventoryBase.SetActive(false);
    }

    public void AcquireItem(Item _item, int _count = 1)
    {
        if (Item.ItemType.ETC != _item.itemType)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item != null)
                {
                    if (slots[i].item.itemName == _item.itemName)
                    {
                        slots[i].SetSlotCount(_count);
                        return;
                    }
                }
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                slots[i].AddItem(_item, _count);
                return;
            }
        }
    }

    public bool HasItem(Item _item, int _count)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item != null && slot.item.itemName == _item.itemName && slot.itemCount >= _count)
            {
                return true;
            }
        }
        return false;
    }

    // Use the required item and count
    public void UseItem(Item _item, int _count)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item != null && slot.item.itemName == _item.itemName)
            {
                slot.SetSlotCount(-_count);
                return;
            }
        }
    }

}
