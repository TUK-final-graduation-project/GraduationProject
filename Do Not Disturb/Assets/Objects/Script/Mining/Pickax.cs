using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickax : MonoBehaviour
{
    [SerializeField]
    PlayerMove player;

    [SerializeField]
    Rock rock;

    private void Start()
    {
      /*  // 필요한 컴포넌트를 초기화
        player = GetComponent<PlayerMove>();
        rock = GetComponent<Rock>();*/
    }

    private void OnCollisionEnter(Collision collision)
    {
        // player와 rock 객체가 null인지 확인.
        if (player == null || rock == null)
        {
            Debug.LogWarning("Player 또는 Rock 컴포넌트가 없습니다.");
            return;
        }

        if (collision.transform.CompareTag("Rock") && player.GetToolIndex() == 1)
        {
            // 충돌한 오브젝트가 Rock 태그를 가지고 있고,
            // 플레이어가 곡괭이를 사용하고 있는 경우에만 실행된다.
            rock.Mining();
        }
    }
}
