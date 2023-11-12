using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

enum AttackType
{
    Normal,
    Skill
}

public class MagicHit : AttackHit
{
    //class HitObject
    //{
    //    public Collider hitObj;
    //    public bool istriggerStay;

    //    public HitObject(Collider hitObj, bool istriggerStay = true)
    //    {
    //        this.hitObj = hitObj;
    //        this.istriggerStay = istriggerStay;
    //    }
    //}

    //List<HitObject> laserHitList = new List<HitObject>();


    [SerializeField] AttackType attackType;
    [SerializeField] LayerMask layerMask;
    bool isHit = false;
    Coroutine CorLaserHit;




    protected override void Start()
    {
        base.Start();
    }

    //레이저 공격
    IEnumerator LaserHit()
    {
        while (true)
        {
            Collider[] hitColliders = Physics.OverlapBox(transform.GetComponent<BoxCollider>().bounds.center, transform.GetComponent<BoxCollider>().size / 2, transform.rotation, layerMask);

            for (int i = 0; i < hitColliders.Length; i++)
            {
                hitColliders[i].GetComponent<Enemy>().Damage(attackPower);
                Debug.Log("Laser attackPower : " + attackPower);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }



    private void OnEnable()
    {
        if (attackType == AttackType.Skill)
            CorLaserHit = StartCoroutine(LaserHit());
    }

    private void OnDisable()
    {
        if (attackType == AttackType.Skill)
            StopCoroutine(CorLaserHit);
    }

    public override void SetAttackPower(int attackPower, float magnifyingPower)
    {
        base.SetAttackPower(attackPower, magnifyingPower);
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.green;
    //    //Debug.Log(transform.GetComponent<BoxCollider>().bounds.size);
    //    Gizmos.DrawCube(
    //        transform.GetComponent<BoxCollider>().bounds.center,
    //        transform.GetComponent<BoxCollider>().size);
    //}

    //기본 공격
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            collision.collider.GetComponent<Enemy>().Damage(attackPower);
            Debug.Log("col attackPower : " + attackPower);
        }
    }

    //protected override void OnTriggerEnter(Collider other)
    //{
    //    base.OnTriggerEnter(other);

    //    if (other.CompareTag("Enemy"))
    //    {
    //        //Debug.Log(other.name);
    //        //istriggerStay = true;
    //        HitObject hitObj = new HitObject(other);
    //        laserHitList.Add(hitObj);
    //        StartCoroutine(LaserDamage(hitObj));
    //    }
    //}

    //private void OnTriggerStay(Collider other)
    //{
    //    Debug.Log("현재 attackPower : " + attackPower);
    //    //if (other.CompareTag("Enemy"))
    //    //{
    //    //    other.GetComponent<Enemy>().Damage(attackPower);
    //    //    Debug.Log("attackPower : " + attackPower);
    //    //}
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("Enemy"))
    //    {
    //        for (int i = 0; i < laserHitList.Count; i++)
    //        {
    //            if (other == laserHitList[i].hitObj)
    //            {
    //                laserHitList[i].istriggerStay = false;
    //                break;
    //            }
    //        }
    //    }

    //}

    //IEnumerator LaserDamage(HitObject other)
    //{
    //    while (other.istriggerStay)
    //    {
    //        other.hitObj.GetComponent<Enemy>().Damage(attackPower);
    //        Debug.Log("attackPower : " + attackPower);

    //        yield return new WaitForSeconds(0.5f);
    //    }

    //    //laserHitList
    //}


}
