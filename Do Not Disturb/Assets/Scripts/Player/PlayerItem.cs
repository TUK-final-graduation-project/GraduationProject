using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerItem : MonoBehaviourPun
{
    [SerializeField]
    private float range; // 습득 가능한 최대 거리.

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
            Debug.LogError("Action Text 오브젝트를 찾을 수 없습니다.");
        }

        inven = FindObjectOfType<Inventory>();
        if (inven == null)
        {
            Debug.LogError("Inventory 컴포넌트를 찾을 수 없습니다.");
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
                Debug.Log(getItem.item.itemName + " 획득했습니다 / 총 " + getItem.item.itemCount + "개");

                if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
                {
                    PhotonView photonView = hitInfo.transform.GetComponent<PhotonView>();
                    if (photonView != null && photonView.ViewID != 0)
                    {
                        photonView.RPC("DestroyItem", RpcTarget.AllBuffered);
                    }
                    else
                    {
                        Debug.LogError("PhotonView가 없는 게임 오브젝트를 제거하려고 시도했습니다: " + hitInfo.transform.name);
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
                Debug.LogWarning("ItemAcquisition 또는 Inventory 컴포넌트를 찾을 수 없습니다.");
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
                + " 획득 " + "<color=yellow>" + "</color>";
            actionText.color = Color.yellow;
        }
    }

    private void InfoDisappear()
    {
        actionText.gameObject.SetActive(false);
    }
}
