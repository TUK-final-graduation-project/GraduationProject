using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    [SerializeField]
    private GameObject tooltipObject;
    [SerializeField]
    private Text tooltipText;

    private void Start()
    {
        HideTooltip();
    }

    private void Update()
    {
        if (tooltipObject.activeSelf)
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                tooltipObject.transform.parent as RectTransform,
                Input.mousePosition,
                null,
                out position
            );
            tooltipObject.transform.localPosition = position;
        }
    }

    public void ShowTooltip(Item item)
    {
        tooltipText.text = $"{item.itemName} {item.itemCount}개"; 
        if (item.itemCount == 0)
        {
            tooltipText.color = Color.red;
        }
        else
        {
            tooltipText.color = Color.white; // 기본 색상
        }
        tooltipObject.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltipObject.SetActive(false);
    }
}
