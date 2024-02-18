using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{

    public Item item; // ȹ���� ������.
    public int itemCount; // ȹ���� �������� ����.
    public Image itemImage; // �������� �̹���.


    // �ʿ��� ������Ʈ.
    [SerializeField]
    private Text text_Count;
    [SerializeField]
    private GameObject go_CountImage;


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

        item = _item;
        itemCount = _count;
        itemImage.sprite = item.itemImage;

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

        SetColor(1);
    }


    // ������ ���� ����.
    public void SetSlotCount(int _count)
    {
        itemCount += _count;
        text_Count.text = itemCount.ToString();

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
