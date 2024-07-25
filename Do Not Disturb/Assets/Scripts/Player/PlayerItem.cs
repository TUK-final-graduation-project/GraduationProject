using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItem : MonoBehaviour
{
    [SerializeField]
    private float range; // ���� ������ �ִ� �Ÿ�.

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
                inven.AcquireItem(getItem.item);
                Debug.Log(getItem.item.itemName + " ȹ���߽��ϴ� / �� " + getItem.item.itemCount + "��");
                Destroy(hitInfo.transform.gameObject);
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Item);
                hitInfo = null; // �������� �ı��� �� hitInfo�� null�� ����
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
            actionText.gameObject.SetActive(true);
            actionText.text = hitInfo.transform.GetComponent<ItemAcquisition>().item.itemName
                + " ȹ�� " + "<color=yellow>" + "</color>";
            actionText.color = Color.yellow;
        }
    }

    private void InfoDisappear()
    {
        actionText.gameObject.SetActive(false);
    }
}
