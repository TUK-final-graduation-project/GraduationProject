using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTurretController : MonoBehaviour
{

    [Header("¹Ì´Ï¾ð")]
    [SerializeField] GameObject Minion;
    [SerializeField] Transform StartPosition;
    [SerializeField] float minion_create_speed;
    [SerializeField] float CurTime;
    [SerializeField] float MaxTime;

    // Start is called before the first frame update
    void Start()
    {
        CurTime = MaxTime;
    }

    // Update is called once per frame
    void Update()
    {
        CurTime -= Time.deltaTime;
        if (CurTime <= 0)
        {
            var a = Instantiate(Minion, StartPosition.position, StartPosition.rotation);
            a.GetComponent<Rigidbody>().AddForce(StartPosition.transform.forward * minion_create_speed);
            Destroy(a.gameObject, 2.0f);
            CurTime = MaxTime;
        }
    }
}
