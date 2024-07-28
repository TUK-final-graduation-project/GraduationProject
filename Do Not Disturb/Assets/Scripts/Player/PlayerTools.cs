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

    bool swapTool1;                 // Ű���� 1 : Į
    bool swapTool2;                 // Ű���� 2 : ���
    bool swapTool3;                 // Ű���� 3 : ����
    bool swapToolNull;              // Ű���� 4 : ����

    bool isSwap;                    // ���� �ٲٴ� �� Ȯ��
    float swapDelay = 0.4f;

    int equipToolIndex = -1;        // ���� �������� ���� ��ȣ 0-2
    int toolIndex = -1;             //

    bool sward;
    bool axe;
    bool pickax;

    bool fDown;                     // ���콺 ��Ŭ��
    bool isSwingReady;              // ���� �غ� �Ϸ�
    float swingDelay;               // ���� ������
    bool isCrafting;

    bool isUIOpen;                  // UI�� ���� �ִ��� Ȯ��

    // Start is called before the first frame update
    void Start()
    {
        isSwingReady = true;
    }

    void FixedUpdate()
    {
        // Ű �Է� ó�� �Լ� ȣ��
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
        // �ߺ���ü ����
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

            // ���� �ִϸ��̼� Ȱ��ȭ
            anim.SetTrigger("Swap");

            isSwap = true;

            // �������� �˸���
            Invoke("SwapOut", swapDelay);
        }

        if (swapToolNull)
        {
            equipTool?.gameObject.SetActive(false);

            // ���� �ִϸ��̼� Ȱ��ȭ
            anim.SetTrigger("Swap");
            isSwap = true;

            // �������� �˸���
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

    // UI ����/�ݱ� �Լ� (UI �Ŵ������� ȣ��)
    public void SetUIOpen(bool isOpen)
    {
        isUIOpen = isOpen;
    }
}
