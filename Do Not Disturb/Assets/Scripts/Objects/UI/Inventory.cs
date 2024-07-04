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

    //public int GetItemCount(string _itemName)
    //{
    //    int temp = SearchSlotItem(slots, _itemName);
    //    return temp != 0 ? temp : SearchSlotItem(quickSlots, _itemName);
    //}

    //private int SearchSlotItem(InventorySlot[] _slots, string _itemName)
    //{
    //    for (int i = 0; i < _slots.Length; i++)
    //    {
    //        if (_slots[i].item != null)
    //        {
    //            if (_itemName == _slots[i].item.itemName)
    //                return _slots[i].itemCount;
    //        }
    //    }

    //    return 0;
    //}

    //public void SetItemCount(string _itemName, int _itemCount)
    //{
    //    if (!ItemCountAdjust(slots, _itemName, _itemCount))
    //        ItemCountAdjust(quickSlots, _itemName, _itemCount);
    //}

    //private bool ItemCountAdjust(InventorySlot[] _slots, string _itemName, int _itemCount)
    //{
    //    for (int i = 0; i < _slots.Length; i++)
    //    {
    //        if (_slots[i].item != null)
    //        {
    //            if (_itemName == _slots[i].item.itemName)
    //            {
    //                _slots[i].SetSlotCount(-_itemCount);
    //                return true;
    //            }
    //        }
    //    }
    //    return false; // �κ��丮�� ��� �����Կ��� ���� ��
    //}
}
