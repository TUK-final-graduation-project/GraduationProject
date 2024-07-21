using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CraftTooltip : MonoBehaviour
{
    [SerializeField] private GameObject tooltipObject;  // 툴팁 UI 패널
    [SerializeField] private Text tooltipText;          // 툴팁 텍스트 컴포넌트

    private void Start()
    {
        HideTooltip();  // 초기에는 툴팁을 숨깁니다.
    }

    // 툴팁을 표시하는 함수
    public void ShowTooltip(string craftName, List<RequiredItem> requiredItems)
    {
        string tooltipContent = craftName + "\n\n\n 필요한 아이템 \n\n";
        foreach (var requiredItem in requiredItems)
        {
            tooltipContent += $"{requiredItem.item.itemName}: {requiredItem.count}\n";
        }
        tooltipText.text = tooltipContent;
        tooltipObject.SetActive(true);
    }

    // 툴팁을 숨기는 함수
    public void HideTooltip()
    {
        tooltipObject.SetActive(false);
    }
}