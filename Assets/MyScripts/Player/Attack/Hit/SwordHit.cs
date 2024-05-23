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

    public override void SetAttackPower(int attackPower, int magnifyingPower)
    {
        base.SetAttackPower(attackPower, magnifyingPower);
        //this.attackPower = (int)(attackPower * magnifyingPower);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<IDamageable>().TakeDamage(damage);
            Debug.Log("damage : " + damage);
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