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

    // Update is called once per frame
    void Update()
    {
        CanPickUp();
        //TryAction();
    }

    private void TryAction()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            CanPickUp();
        }
    }

    private void CanPickUp()
    {
        if (pickupActivated && hitInfo.transform != null)
        {
            ItemAcquisition getItem = hitInfo.transform.GetComponent<ItemAcquisition>();
            if (getItem != null && inven != null)
            {
                Debug.Log(getItem.item.itemName + " ȹ���߽��ϴ�");
                inven.AcquireItem(getItem.item);
                Destroy(hitInfo.transform.gameObject);
                InfoDisappear();
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Item")
        {
            hitInfo = other;
            ItemInfoAppear();
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

    private void ItemInfoAppear()
    {
        pickupActivated = true;
        actionText.gameObject.SetActive(true);
        actionText.text = hitInfo.transform.GetComponent<ItemAcquisition>().item.itemName
            + " ȹ�� " + "<color=yellow>" + "(E)" + "</color>";
    }
    private void InfoDisappear()
    {
        pickupActivated = false;
        actionText.gameObject.SetActive(false);
    }
}
