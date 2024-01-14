/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolManager : MonoBehaviour
{
    // ���� �ߺ� ��ü ���� ����
    public static bool isChangeTool = false;

    // ��ü ������ , ���� ��ü�� ������ ���� ����
    [SerializeField]
    private float changeToolDelayTime;
    [SerializeField]
    private float changeToolEndDelayTime;

    // ���� ������ ���� ����
    [SerializeField]
    private Tool[] tools;
    [SerializeField]
    private Hand[] hands;

    // ���� ���� ������ �����ϵ��� ���� �����
    private Dictionary<string, Tool> toolDictionary = new Dictionary<string, Tool>();
    private Dictionary<string, Hand> handDictionary = new Dictionary<string, Hand>();

    [SerializeField]
    private string toolController;
    [SerializeField]
    private string handController;


    // ���� ������ Ÿ��
    [SerializeField]
    private string currentToolType;

    public static Transform currentTool;

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < tools.Length; i++)
        {
            toolDictionary.Add(tools[i].toolName, tools[i]);
        }
        for (int i = 0; i < hands.Length; i++)
        {
            handDictionary.Add(hands[i].handName, hands[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isChangeTool)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
                StartCoroutine(ChangeToolCoroutine("HAND", "�Ǽ�"));
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                StartCoroutine(ChangeToolCoroutine("TOOL", "SubMachineTool1"));
        }
    }

    // ���� ��ü �ڷ�ƾ.
    public IEnumerator ChangeToolCoroutine(string _type, string _name)
    {
        isChangeTool = true;
        currentToolAnim.SetTrigger("Tool_Out");

        yield return new WaitForSeconds(changeToolDelayTime);

        CancelPreToolAction();
        ToolChange(_type, _name);

        yield return new WaitForSeconds(changeToolEndDelayTime);

        currentToolType = _type;
        isChangeTool = false;
    }

    // ���� ��� �Լ�.
    private void CancelPreToolAction()
    {
        switch (currentToolType)
        {
            case "TOOL":
                theToolController.CancelFineSight();
                theToolController.CancelReload();
                ToolController.isActivate = false;
                break;
            case "HAND":
                HandController.isActivate = false;
                break;
        }
    }

    // ���� ��ü �Լ�.
    private void ToolChange(string type, string name)
    {
        if (type == "TOOL")
            theToolController.ToolChange(toolDictionary[name]);
        else if (type == "HAND")
            theHandController.HandChange(handDictionary[name]);
    }
}*/