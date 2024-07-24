using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.Image;

public class Tower : MonoBehaviour
{
    public enum Type { Wide, Focus, Blade }

    public Type type;
    public int HP;          // 생명력
    public int Def;         // 방어력
    public float attackSpeed;    // 공격 속도
    public bool isConquer = false;
    public GameObject releasePoint;

    public GameObject grenadeObject;
    public bool isAttack = false;

    public GameObject rotateObj;
    GameObject target;

    void Targeting()
    {
        RaycastHit[] rayHits = { };
        Vector3 dir = new Vector3(0, 0, 1);

        rayHits =
            Physics.SphereCastAll(transform.position, 15, Vector3.up, 0, LayerMask.GetMask("Com"));

        if (rayHits.Length > 0 && !isAttack)
        {
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
                {
                    yield return new WaitForSeconds(0.1f);
                    rotateObj.GetComponent<Rotate>().isRotate = false;
                    if (target != null)
                    {
                        Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
                        rotateObj.transform.rotation = new Quaternion(0, targetRotation.y, 0, targetRotation.w);
                        Focus();
                    }
                    yield return new WaitForSeconds(0.5f);
                    rotateObj.GetComponent<Rotate>().isRotate = true;
                    break;
                }
            case Type.Wide:
                {
                    yield return new WaitForSeconds(0.1f);
                    rotateObj.GetComponent<Rotate>().isRotate = false;
                    Quaternion targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
                    rotateObj.transform.rotation = new Quaternion(0, targetRotation.y, 0, targetRotation.w);
                    // 데모를 위한 block
                    Wide();
                    yield return new WaitForSeconds(10f);
                    rotateObj.GetComponent<Rotate>().isRotate = true;
                    break;
                }
            case Type.Blade:
                yield return new WaitForSeconds(0.1f);
                Blade();
                yield return new WaitForSeconds(0.1f);
                break;
        }
        isAttack = false;
        transform.rotation = Quaternion.identity;
    }

    void Wide()
    {

        Vector3 nextVec = releasePoint.transform.forward;
        nextVec.Normalize();
        // nextVec.y = 10;

        GameObject instantGrenade = Instantiate(grenadeObject, releasePoint.transform.position, transform.rotation);
        Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
        rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
        //rigidGrenade.AddTorque(Vector3.back, ForceMode.Impulse);
        //rigidGrenade.velocity = nextVec * 2;
    }

    void Focus() 
    {
        Vector3 nextVec = releasePoint.transform.forward;
        nextVec.Normalize();
        // nextVec.y = 10;
        GameObject instantGrenade = Instantiate(grenadeObject, releasePoint.transform.position, transform.rotation);
        instantGrenade.GetComponent<SlowBullet>().TargetPosition = target.transform.position;
        //Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
        //rigidGrenade.AddForce(nextVec, ForceMode.Impulse);
        ////rigidGrenade.AddTorque(Vector3.back, ForceMode.Impulse);
        //rigidGrenade.velocity = nextVec * 2;
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

            Destroy(transform.parent.gameObject, 5);
        }
    }
    public void SetHP(int hp)
    {
        HP = hp;
    }
    public void SetAttackSpeed(float speed)
    {
        attackSpeed = speed;
    }
    public void SetDef(int def)
    {
        Def = def;
    }


}
