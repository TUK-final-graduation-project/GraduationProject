using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTowerController : MonoBehaviour
{
    [Header("UNIT")]
    [SerializeField] GameObject Fire_Unit;
    [SerializeField] GameObject Water_Unit;
    [SerializeField] GameObject Metal_Unit;
    [SerializeField] GameObject Tree_Unit;
    [SerializeField] Transform StartPosition;
    [SerializeField] float minion_create_speed;
    [SerializeField] float CurTime;
    [SerializeField] float MaxTime;

    GameObject Minion;
    
    // Start is called before the first frame update
    void Start()
    {
        CurTime = MaxTime;
        Minion = Fire_Unit;
    }

    // Update is called once per frame
    void Update()
    {
        CurTime -= Time.deltaTime;
        if (CurTime <= 0)
        {
            var a = Instantiate(Minion, StartPosition.position, StartPosition.rotation);
            a.GetComponent<Rigidbody>().AddForce(StartPosition.transform.forward * minion_create_speed);
            // Destroy(a.gameObject, 2.0f);
            CurTime = MaxTime;
        }
    }
}
