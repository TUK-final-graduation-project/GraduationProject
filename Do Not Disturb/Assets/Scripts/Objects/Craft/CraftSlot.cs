using UnityEngine;
using UnityEngine.EventSystems;

public class CraftSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private int slotNumber;
    private CraftMenu craftMenu;
    private CraftTooltip tooltip;

    void Start()
    {
        craftMenu = FindObjectOfType<CraftMenu>();
        tooltip = craftMenu.tooltip; // CraftMenu���� ���� ���� ��������
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Craft craft = craftMenu.GetCraft(slotNumber);
        if (craft != null)
        {
            tooltip.ShowTooltip(craft, craftMenu.GetDiscountRate()); // ���ΰ� ����
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.HideTooltip();
    }
}
