using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItem : MonoBehaviour
{
    [SerializeField]
    private float range; // 습득 가능한 최대 거리.

    // 아이템 레이어에만 반응하도록 레이어 마스크를 설정.
    [SerializeField]
    private LayerMask layerMask;

    // 필요한 컴포넌트.
    [SerializeField]
    private Text actionText;
    [SerializeField]
    private Inventory inven;

    private Collider hitInfo; // 충돌체 정보 저장.

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Item")
        {
            hitInfo = other;
            ItemInfoAppear();
            Invoke("InfoDisappear", 2.0f);

            // 아이템 획득
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
                Destroy(hitInfo.transform.gameObject);
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Item);
                hitInfo = null; // 아이템을 파괴한 후 hitInfo를 null로 설정
            }
            else
            {
                Debug.LogWarning("ItemAcquisition 또는 Inventory 컴포넌트를 찾을 수 없습니다.");
            }
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
