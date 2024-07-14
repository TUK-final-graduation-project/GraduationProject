using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItem : MonoBehaviour
{
    [SerializeField]
    private float range; // ���� ������ �ִ� �Ÿ�.

    private bool pickupActivated = false; // ���� ������ �� true.

    // ������ ���̾�� �����ϵ��� ���̾� ����ũ�� ����.
    [SerializeField]
    private LayerMask layerMask;

    // �ʿ��� ������Ʈ.
    [SerializeField]
    private Text actionText;
    [SerializeField]
    private Inventory inven;

    private Collider hitInfo; // �浹ü ���� ����.

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Item")
        {
            hitInfo = other;
            ItemInfoAppear();
            Invoke("InfoDisappear", 2.0f);

            // ������ ȹ��
            CanPickUp();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            hitInfo = null;
            InfoDisappear();
        }
    }

    private void CanPickUp()
    {
        if (hitInfo != null)
        {
            ItemAcquisition getItem = hitInfo.transform.GetComponent<ItemAcquisition>();
            if (getItem != null && inven != null)
            {
                Debug.Log(getItem.item.itemName + " ȹ���߽��ϴ� / �� " + getItem.item.itemCount + "��");
                inven.AcquireItem(getItem.item);
                Destroy(hitInfo.transform.gameObject);
                hitInfo = null; // �������� �ı��� �� hitInfo�� null�� ����
                InfoDisappear(); // ������ ȹ�� �� ���� ����
            }
            else
            {
                Debug.LogWarning("ItemAcquisition �Ǵ� Inventory ������Ʈ�� ã�� �� �����ϴ�.");
            }
        }
    }

    private void ItemInfoAppear()
    {
        if (hitInfo != null && hitInfo.transform.GetComponent<ItemAcquisition>() != null)
        {
            pickupActivated = true;
            actionText.gameObject.SetActive(true);
            actionText.text = hitInfo.transform.GetComponent<ItemAcquisition>().item.itemName
                + " ȹ�� " + "<color=yellow>" + "</color>";
        }
    }

    private void InfoDisappear()
    {
        pickupActivated = false;
        actionText.gameObject.SetActive(false);
    }
}
