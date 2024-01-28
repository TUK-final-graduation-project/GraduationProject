using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSAstar : MonoBehaviour
{
    [SerializeField]
    private Vector3 GoalPoint;
    private Vector3 Dir;

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(GoalPoint.x, GoalPoint.y, GoalPoint.z), Time.deltaTime * 2f);
    }
}
