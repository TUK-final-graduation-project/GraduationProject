using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerItem : MonoBehaviourPun
{
    [SerializeField]
    private float range; // ���� ������ �ִ� �Ÿ�.

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private Text actionText;
    [SerializeField]
    private Inventory inven;

    private Collider hitInfo;

    void Start()
    {
        GameObject actionTextObject = GameObject.Find("Action Text");
        if (actionTextObject != null)
        {
            actionText = actionTextObject.GetComponent<Text>();
        }
        else
        {
            Debug.LogError("Action Text ������Ʈ�� ã�� �� �����ϴ�.");
        }

        inven = FindObjectOfType<Inventory>();
        if (inven == null)
        {
            Debug.LogError("Inventory ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Item")
        {
            hitInfo = other;
            ItemInfoAppear();
            Invoke("InfoDisappear", 2.0f);

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

                if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
                {
                    PhotonView photonView = hitInfo.transform.GetComponent<PhotonView>();
                    if (photonView != null && photonView.ViewID != 0)
                    {
                        photonView.RPC("DestroyItem", RpcTarget.AllBuffered);
                    }
                    else
                    {
                        Debug.LogError("PhotonView�� ���� ���� ������Ʈ�� �����Ϸ��� �õ��߽��ϴ�: " + hitInfo.transform.name);
                        Destroy(hitInfo.transform.gameObject);
                    }
                }
                else
                {
                    Destroy(hitInfo.transform.gameObject);
                }

                hitInfo = null;
            }
            else
            {
                Debug.LogWarning("ItemAcquisition �Ǵ� Inventory ������Ʈ�� ã�� �� �����ϴ�.");
            }
        }
    }

    [PunRPC]
    private void DestroyItem()
    {
        if (hitInfo != null)
        {
            Destroy(hitInfo.transform.gameObject);
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
