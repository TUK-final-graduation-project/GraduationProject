using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CraftTooltip : MonoBehaviour
{
    [SerializeField] private GameObject tooltip;
    [SerializeField] private Text craftNameText;
    [SerializeField] private Text requiredItemsText;

    private void Start()
    {
        HideTooltip();  // √ ±‚ø°¥¬ ≈¯∆¡¿ª º˚±È¥œ¥Ÿ.
    }
    public void ShowTooltip(Craft craft, float discountRate)
    {
        craftNameText.text = craft.craftName;

        requiredItemsText.text = "";
        foreach (var requiredItem in craft.requiredItems)
        {
            int discountedCount = Mathf.CeilToInt(requiredItem.count * (1 - discountRate));
            string itemText = $"{requiredItem.item.itemName} x {requiredItem.count}";

            if (discountRate > 0)
            {
                itemText += $" («“¿Œ∞°: <color=yellow>{discountedCount}</color>)";
            }

            requiredItemsText.text += itemText + "\n";
        }

        tooltip.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltip.SetActive(false);
    }
}
