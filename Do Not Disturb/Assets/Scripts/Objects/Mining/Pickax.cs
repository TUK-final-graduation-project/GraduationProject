using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickax : MonoBehaviour
{
    [SerializeField]
    PlayerTools player;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("PlayerTools�� �Ҵ���� �ʾҽ��ϴ�.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // player�� rock ��ü�� null���� Ȯ��.
        if (player == null)
        {
            Debug.LogWarning("Player �Ǵ� Rock ������Ʈ�� �����ϴ�.");
            return;
        }

        // �浹�� ������Ʈ�� Rock �±׸� ������ �ְ�,
        // �÷��̾ ��̸� ����ϰ� �ִ� ��쿡�� ����ȴ�.
        if (other.transform.tag == "Rock" && player.GetToolIndex() == 1)
        {
            Rock rock = other.gameObject.GetComponent<Rock>();
            if (rock != null)
            {
                rock.Mining();
            }
            else
            {
                Debug.LogWarning("Rock ������Ʈ�� ã�� �� �����ϴ�.");
            }
        }
    }
}
