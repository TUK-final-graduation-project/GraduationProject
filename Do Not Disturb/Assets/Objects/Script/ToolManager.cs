/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolManager : MonoBehaviour
{
    // 도구 중복 교체 실행 방지
    public static bool isChangeTool = false;

    // 교체 딜레이 , 도구 교체가 완전히 끝난 시점
    [SerializeField]
    private float changeToolDelayTime;
    [SerializeField]
    private float changeToolEndDelayTime;

    // 도구 종류들 전부 관리
    [SerializeField]
    private Tool[] tools;
    [SerializeField]
    private Hand[] hands;

    // 쉽게 도구 접근이 가능하도록 사전 만들기
    private Dictionary<string, Tool> toolDictionary = new Dictionary<string, Tool>();
    private Dictionary<string, Hand> handDictionary = new Dictionary<string, Hand>();

    [SerializeField]
    private string toolController;
    [SerializeField]
    private string handController;


    // 현재 도구의 타입
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
                StartCoroutine(ChangeToolCoroutine("HAND", "맨손"));
            else if (Input.GetKeyDown(KeyCode.Alpha2))
                StartCoroutine(ChangeToolCoroutine("TOOL", "SubMachineTool1"));
        }
    }

    // 도구 교체 코루틴.
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

    // 도구 취소 함수.
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

    // 도구 교체 함수.
    private void ToolChange(string type, string name)
    {
        if (type == "TOOL")
            theToolController.ToolChange(toolDictionary[name]);
        else if (type == "HAND")
            theHandController.HandChange(handDictionary[name]);
    }
}*/