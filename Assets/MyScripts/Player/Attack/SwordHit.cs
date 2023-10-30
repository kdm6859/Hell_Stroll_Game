using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHit : AttackHit
{
    protected override void Start()
    {
        base.Start();

        Destroy(gameObject, 1f);
    }

    public void SetAttackPower(int attackPower)
    {
        this.attackPower = (int)(attackPower * magnifyingPower);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().Damage(attackPower);
            Debug.Log("attackPower : " + attackPower);
        }
    }

    //protected override void OnParticleCollision(GameObject other)
    //{
    //    base.OnParticleCollision(other);

    //    if (other.layer == enemyLayer)
    //    {
    //        other.GetComponent<Enemy>().Damage();
    //        //Destroy(other);
    //    }

    //}

    //protected override void OnParticleTrigger()
    //{
    //    base.OnParticleTrigger();

    //}
}
