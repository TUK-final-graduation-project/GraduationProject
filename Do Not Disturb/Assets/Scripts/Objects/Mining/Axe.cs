using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : MonoBehaviour
{
    [SerializeField]
    PlayerTools player;

    private void OnTriggerEnter(Collider other)
    {
        // player�� rock ��ü�� null���� Ȯ��.
        if (player == null)
        {
            Debug.LogWarning("Player �Ǵ� �ڿ� ������Ʈ�� �����ϴ�.");
            return;
        }

        // �浹�� ������Ʈ�� Rock �±׸� ������ �ְ�,
        // �÷��̾ ������ ����ϰ� �ִ� ��쿡�� ����ȴ�.
        if (other.transform.tag == "Tree" && player.GetToolIndex() == 2)
            other.gameObject.GetComponent<Rock>().Mining();
    }
}
