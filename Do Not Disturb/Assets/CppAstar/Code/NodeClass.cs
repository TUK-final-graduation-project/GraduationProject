using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeClass
{
    public bool isWalkable;                     // �� �� �ִ� ������ ���� �Ǵ�
    public Vector3 worldPosition;               // ����� ���� ��ǥ ����

    public NodeClass(bool n_isWalkable, Vector3 n_WorldPos)
    {
        isWalkable = n_isWalkable;
        worldPosition = n_WorldPos;
    }
}
