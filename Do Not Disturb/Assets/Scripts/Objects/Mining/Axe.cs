using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{
    [SerializeField]
    PlayerTools player;

    private void OnTriggerEnter(Collider other)
    {
        // player와 rock 객체가 null인지 확인.
        if (player == null)
        {
            Debug.LogWarning("Player 또는 자원 컴포넌트가 없습니다.");
            return;
        }

        // 충돌한 오브젝트가 Rock 태그를 가지고 있고,
        // 플레이어가 도끼를 사용하고 있는 경우에만 실행된다.
        if (other.transform.tag == "Tree" && player.GetToolIndex() == 2)
            other.gameObject.GetComponent<Rock>().Mining();
    }
}
