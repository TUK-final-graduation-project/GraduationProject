using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [Header("Fight")]
    // public GameObject player;
    public GameObject bullet;
    [SerializeField] float maxTime = 3f;
    [SerializeField] float curTime = 1f;
    public float bulletSpeed = 30f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (curTime <= 0)
        {
            // Vector3 dir = (player.transform.position - transform.position).normalized;
            Vector3 dir = (new Vector3(0f, 0f, 0f) - transform.position).normalized;
            Debug.Log(dir);
            var a = Instantiate(bullet, transform.position, Quaternion.identity);
            a.GetComponent<Rigidbody>().AddForce(dir * bulletSpeed, ForceMode.Impulse);
            curTime = maxTime;

        }
        curTime -= Time.deltaTime;
    }
}
