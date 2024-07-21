using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CraftTooltip : MonoBehaviour
{
    [SerializeField] private GameObject tooltipObject;  // ���� UI �г�
    [SerializeField] private Text tooltipText;          // ���� �ؽ�Ʈ ������Ʈ

    private void Start()
    {
        HideTooltip();  // �ʱ⿡�� ������ ����ϴ�.
    }

    // ������ ǥ���ϴ� �Լ�
    public void ShowTooltip(string craftName, List<RequiredItem> requiredItems)
    {
        string tooltipContent = craftName + "\n\n\n �ʿ��� ������ \n\n";
        foreach (var requiredItem in requiredItems)
        {
            tooltipContent += $"{requiredItem.item.itemName}: {requiredItem.count}\n";
        }
        tooltipText.text = tooltipContent;
        tooltipObject.SetActive(true);
    }

    // ������ ����� �Լ�
    public void HideTooltip()
    {
        tooltipObject.SetActive(false);
    }
}