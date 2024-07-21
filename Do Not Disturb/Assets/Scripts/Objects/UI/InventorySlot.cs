using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Item item; // ȹ���� ������.
    public int itemCount; // ȹ���� �������� ����.
    public Image itemImage; // �������� �̹���.

    // �ʿ��� ������Ʈ.
    [SerializeField]
    private Text text_Count;
    [SerializeField]
    private GameObject go_CountImage;

    // Tooltip reference
    private Tooltip tooltip;

    private void Awake()
    {
        tooltip = FindObjectOfType<Tooltip>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null && tooltip != null)
        {
            Debug.Log($"Showing tooltip for item: {item.itemName}");
            tooltip.ShowTooltip(item);
        }
        else
        {
            Debug.Log("Item or tooltip is null.");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltip != null)
        {
            Debug.Log("Hiding tooltip.");
            tooltip.HideTooltip();
        }
        else
        {
            Debug.Log("Tooltip is null.");
        }
    }


    // �̹����� ���� ����.
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // ������ ȹ��
    public void AddItem(Item _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.itemImage;

        if (_item == null)
        {
            Debug.LogError("�������� null�Դϴ�.");
            return;
        }

        if (itemImage == null)
        {
            Debug.LogError("itemImage�� null�Դϴ�.");
            return;
        }

        if (go_CountImage == null)
        {
            Debug.LogError("go_CountImage�� null�Դϴ�.");
            return;
        }

        if (text_Count == null)
        {
            Debug.LogError("text_Count�� null�Դϴ�.");
            return;
        }

        if (item.itemType != Item.ItemType.ETC)
        {
            go_CountImage.SetActive(true);
            text_Count.text = itemCount.ToString();
        }
        else
        {
            text_Count.text = "0";
            go_CountImage.SetActive(false);
        }
        item.itemCount = itemCount;
        SetColor(1);
    }


    // ������ ���� ����.
    public void SetSlotCount(int count)
    {
        itemCount += count;
        text_Count.text = itemCount.ToString();
        item.itemCount = itemCount;
        Debug.Log("count : " + itemCount);
        if (itemCount <= 0)
            ClearSlot();
    }

    // ���� �ʱ�ȭ.
    private void ClearSlot()
    {
        item = null;
        itemCount = 0;
        itemImage.sprite = null;
        SetColor(0);

        text_Count.text = "0";
        go_CountImage.SetActive(false);
    }
}
