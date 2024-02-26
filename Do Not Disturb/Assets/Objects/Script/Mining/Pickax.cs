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
      /*  // �ʿ��� ������Ʈ�� �ʱ�ȭ
        player = GetComponent<PlayerMove>();
        rock = GetComponent<Rock>();*/
    }

    private void OnCollisionEnter(Collision collision)
    {
        // player�� rock ��ü�� null���� Ȯ��.
        if (player == null || rock == null)
        {
            Debug.LogWarning("Player �Ǵ� Rock ������Ʈ�� �����ϴ�.");
            return;
        }

        if (collision.transform.CompareTag("Rock") && player.GetToolIndex() == 1)
        {
            // �浹�� ������Ʈ�� Rock �±׸� ������ �ְ�,
            // �÷��̾ ��̸� ����ϰ� �ִ� ��쿡�� ����ȴ�.
            rock.Mining();
        }
    }
}
