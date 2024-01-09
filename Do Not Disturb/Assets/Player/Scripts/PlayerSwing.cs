using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwing : MonoBehaviour
{
    public string hodingToolName;   // ���� ��� �ִ� ���� �̸�

    // ���� ����
    public bool isHand;             // �� ��
    public bool isAxe;              // ����
    public bool isPickax;           // ���

    public float range;             // Swing ����
    public float speed;             // Swing �ӵ�
    public int damage;              // Swing ���ݷ�

    public float delayTime;         // ������ �ð�
    public float delayTA;           // Swing Ȱ��ȭ ����
    public float delayTB;           // Swing ��Ȱ��ȭ ����

    public Animator anim;           // �ִϸ��̼�
}
