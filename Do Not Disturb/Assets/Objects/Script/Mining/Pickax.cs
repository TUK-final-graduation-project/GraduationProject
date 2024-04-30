using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickax : MonoBehaviour
{
    [SerializeField]
    PlayerMove player;

    //[SerializeField]
    //Rock rock;

    private void Awake()
    {
        // �ʿ��� ������Ʈ�� �ʱ�ȭ ....���� �ʿ�(2/29)
        //player = GetComponent<PlayerMove>();
        //rock = GetComponent<Rock>();
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
            other.gameObject.GetComponent<Rock>().Mining();
            //rock.Mining();
    }
}
