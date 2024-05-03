using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class Tower : MonoBehaviour
{
    public enum Type { Wide, Focus, Blade }

    public Type type;
    public int HP;
    public bool isConquer = false;
    public GameObject releasePoint;

    public GameObject grenadeObject;
    public bool isAttack = false;

    GameObject target;
    private void Update()
    {

    }


    void Targeting()
    {
        float targetRadius = 10f;
        float targetRange = 70f;

        RaycastHit[] rayHits = { };
        Vector3 dir = new Vector3(0, 0, 1);

        rayHits =
            Physics.SphereCastAll(transform.position, 15, Vector3.up, 0, LayerMask.GetMask("Com"));

        if (rayHits.Length > 0 && !isAttack)
        {
            Debug.Log("Hit");
            target = rayHits[0].collider.gameObject;
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {

        isAttack = true;
        switch (type)
        {
            case Type.Focus:
                yield return new WaitForSeconds(0.1f);
                Focus();
                yield return new WaitForSeconds(0.5f);
                break;
            case Type.Wide:
                yield return new WaitForSeconds(0.1f);
                Wide();
                yield return new WaitForSeconds(10f);
                break;
            case Type.Blade:
                yield return new WaitForSeconds(0.1f);
                Blade();
                yield return new WaitForSeconds(0.1f);
                break;
        }
        isAttack = false;
    }

    void Wide()
    {
        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
        targetRotation = new Quaternion(0, targetRotation.y, 0, 0);
        releasePoint.transform.parent.transform.rotation = targetRotation;
        Vector3 nextVec = target.transform.position - transform.position;
        nextVec.Normalize();
        nextVec.y = 10;

        GameObject instantGrenade = Instantiate(grenadeObject, releasePoint.transform.position, transform.rotation);
        Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
        rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
        //rigidGrenade.AddTorque(Vector3.back, ForceMode.Impulse);
        //rigidGrenade.velocity = nextVec * 2;
    }

    void Focus() 
    { 

    }

    void Blade()
    {

    }
    private void FixedUpdate()
    {
        Targeting();
    }
    public void OnDamage()
    {
        HP -= 50;
        if ( HP < 0 )
        {
            isConquer = true;
        }
    }
}
