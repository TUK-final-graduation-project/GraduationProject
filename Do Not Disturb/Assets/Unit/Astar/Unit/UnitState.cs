using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

public struct Pos
{
    public Vector3 locate;
    public bool isFull;
}

public class UnitState : MonoBehaviour
{
    public Pos[] nearPosition;

    private void Start()
    {
        nearPosition = new Pos[10];   
    }

    public int HP = 10000000;
    public bool isDead = false;
    public void UnderAttack(int damage)
    {
        if(HP > 0)
        {
            HP -= damage;
        }
        else
        {
            isDead = true;
        }
    }
    private void Update()
    {
        if (isDead)
        {
            Destroy(gameObject);
        }
    }
    public Pos[] GetNearPosition()
    {
        Vector3 curPos = gameObject.transform.position;

        float angleIncrement = 360f / 10f; // 각도 증분
        float radius = 2f; // 원의 반지름

        for ( int i = 0; i < 10; ++i)
        {
            float angle = Mathf.Deg2Rad * angleIncrement * i;

            float x = curPos.x + radius * Mathf.Cos(angle);
            float z = curPos.z + radius * Mathf.Sin(angle);

            nearPosition[i].locate = new Vector3 ( x, 0, z);

            Debug.Log( i + " " + nearPosition[i].locate);
        }
        return nearPosition;
    }
    void SetTargeting()
    {

    }
}
