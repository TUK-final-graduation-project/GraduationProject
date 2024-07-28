using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;

public class PlayerTools : MonoBehaviourPun
{
    public GameObject[] tools;
    public bool[] hasTools;
    Tools equipTool;
    GameManager manager;

    [SerializeField]
    Animator anim;

    bool swapTool1;                 // 키보드 1 : 칼
    bool swapTool2;                 // 키보드 2 : 곡괭이
    bool swapTool3;                 // 키보드 3 : 도끼
    bool swapToolNull;              // 키보드 4 : 리셋

    bool isSwap;                    // 도구 바꾸는 중 확인
    float swapDelay = 0.4f;

    int equipToolIndex = -1;        // 현재 장착중인 도구 번호 0-2
    int toolIndex = -1;             //

    bool sward;
    bool axe;
    bool pickax;

    bool fDown;                     // 마우스 좌클릭
    bool isSwingReady;              // 스윙 준비 완료
    float swingDelay;               // 스윙 딜레이
    bool isCrafting;

    bool isUIOpen;                  // UI가 열려 있는지 확인

    // Start is called before the first frame update
    void Start()
    {
        isSwingReady = true;
    }

    void FixedUpdate()
    {
        // 키 입력 처리 함수 호출
        HandleInput();
    }

    void HandleInput()
    {
        if (!photonView.IsMine) return;

        fDown = Input.GetButton("Fire1");

        swapTool1 = Input.GetButton("Swap1");
        swapTool2 = Input.GetButton("Swap2");
        swapTool3 = Input.GetButton("Swap3");
        swapToolNull = Input.GetButton("ToolReset");
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) return;

        Swap();
        Swing();
    }

    void Swing()
    {
        if (equipTool == null || isUIOpen)
            return;

        swingDelay += Time.deltaTime;
        isSwingReady = equipTool.rate < swingDelay;

        if (fDown && isSwingReady && !isSwap)
        {
            equipTool.Use();

            Debug.Log("Swing");
            if (sward)
            {
                anim.SetTrigger("Sward");
                //AudioManager.instance.PlaySfx(AudioManager.Sfx.Shovel);
            }
            if (axe)
            {
                anim.SetTrigger("Axe");
                //AudioManager.instance.PlaySfx(AudioManager.Sfx.Chopping);
            }
            if (pickax)
            {
                anim.SetTrigger("Pickax");
                //AudioManager.instance.PlaySfx(AudioManager.Sfx.Mining);
            }
            swingDelay = 0;
        }
    }

    void Swap()
    {
        // 중복교체 막음
        if (swapTool1 && (!hasTools[0] || equipToolIndex == 0))
            return;
        if (swapTool2 && (!hasTools[1] || equipToolIndex == 1))
            return;
        if (swapTool3 && (!hasTools[2] || equipToolIndex == 2))
            return;
        if (swapToolNull && equipToolIndex == -1)
            return;

        toolIndex = -1;
        if (swapTool1) { toolIndex = 0; sward = true; axe = false; pickax = false; }
        if (swapTool2) { toolIndex = 1; sward = false; axe = false; pickax = true; }
        if (swapTool3) { toolIndex = 2; sward = false; axe = true; pickax = false; }
        if (swapToolNull) { toolIndex = -1; sward = false; axe = false; pickax = false; }

        if ((swapTool1 || swapTool2 || swapTool3))
        {
            equipTool?.gameObject.SetActive(false);

            equipToolIndex = toolIndex;
            equipTool = tools[toolIndex].GetComponent<Tools>();
            equipTool.gameObject.SetActive(true);

            // 장착 애니메이션 활성화
            anim.SetTrigger("Swap");

            isSwap = true;

            // 스왑종료 알리기
            Invoke("SwapOut", swapDelay);
        }

        if (swapToolNull)
        {
            equipTool?.gameObject.SetActive(false);

            // 해제 애니메이션 활성화
            anim.SetTrigger("Swap");
            isSwap = true;

            // 스왑종료 알리기
            Invoke("SwapOut", swapDelay);
        }
    }

    void SwapOut()
    {
        Debug.Log("SwapOut");
        isSwap = false;
    }

    public int GetToolIndex()
    {
        return equipToolIndex;
    }

    // UI 열기/닫기 함수 (UI 매니저에서 호출)
    public void SetUIOpen(bool isOpen)
    {
        isUIOpen = isOpen;
    }
}
