using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Item item; // 획득한 아이템.
    public int itemCount; // 획득한 아이템의 개수.
    public Image itemImage; // 아이템의 이미지.

    // 필요한 컴포넌트.
    [SerializeField]
    private Text text_Count;
    [SerializeField]
    private GameObject go_CountImage;

    // InventoryTooltip reference
    private InventoryTooltip tooltip;

    private void Awake()
    {
        tooltip = FindObjectOfType<InventoryTooltip>();
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


    // 이미지의 투명도 조절.
    private void SetColor(float _alpha)
    {
        Color color = itemImage.color;
        color.a = _alpha;
        itemImage.color = color;
    }

    // 아이템 획득
    public void AddItem(Item _item, int _count = 1)
    {
        item = _item;
        itemCount = _count;
        itemImage.sprite = item.itemImage;

        if (_item == null)
        {
            Debug.LogError("아이템이 null입니다.");
            return;
        }

        if (itemImage == null)
        {
            Debug.LogError("itemImage가 null입니다.");
            return;
        }

        if (go_CountImage == null)
        {
            Debug.LogError("go_CountImage가 null입니다.");
            return;
        }

        if (text_Count == null)
        {
            Debug.LogError("text_Count가 null입니다.");
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


    // 아이템 개수 조정.
    public void SetSlotCount(int count)
    {
        itemCount += count;
        text_Count.text = itemCount.ToString();
        item.itemCount = itemCount;
        Debug.Log("count : " + itemCount);
        if (itemCount <= 0)
            ClearSlot();
    }

    // 슬롯 초기화.
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
