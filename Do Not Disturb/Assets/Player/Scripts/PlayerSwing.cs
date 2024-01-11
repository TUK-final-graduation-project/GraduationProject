using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwing : MonoBehaviour
{
    public string hodingToolName;   // 현재 들고 있는 도구 이름

    // 도구 유형
    public bool isHand;             // 맨 손
    public bool isAxe;              // 도끼
    public bool isPickax;           // 곡괭이

    public float range;             // Swing 범위
    public float speed;             // Swing 속도
    public int damage;              // Swing 공격력

    public float delayTime;         // 딜레이 시간
    public float delayTA;           // Swing 활성화 시점
    public float delayTB;           // Swing 비활성화 시점

    public Animator anim;           // 애니메이션
}
