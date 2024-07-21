using UnityEngine;
using UnityEngine.EventSystems;

public class CraftSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int slotIndex;  // ���� �ε���
    private CraftMenu craftMenu;  // CraftMenu ��ũ��Ʈ ����

    private void Awake()
    {
        craftMenu = FindObjectOfType<CraftMenu>();
        if (craftMenu == null)
        {
            Debug.LogError("CraftMenu not found in the scene.");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (craftMenu != null)
        {
            Craft currentCraft = craftMenu.GetCraft(slotIndex);
            if (currentCraft != null && craftMenu.tooltip != null)
            {
                craftMenu.tooltip.ShowTooltip(currentCraft.craftName, currentCraft.requiredItems);
            }
            else
            {
                Debug.LogWarning("CurrentCraft or Tooltip is null.");
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (craftMenu != null && craftMenu.tooltip != null)
        {
            craftMenu.tooltip.HideTooltip();
        }
    }
}
