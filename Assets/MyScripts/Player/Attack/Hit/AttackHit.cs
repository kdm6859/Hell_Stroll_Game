using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackHit : MonoBehaviour
{
    protected const int enemyLayer = 6;

    protected int damage = 1;

    protected virtual void Start()
    {
        
    }

    public virtual void SetAttackPower(int attackPower, int magnifyingPower)
    {
        this.damage = (int)(attackPower * magnifyingPower * 0.01f);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {

    }


    //protected virtual void OnParticleCollision(GameObject other)
    //{

    //}

    //protected virtual void OnParticleTrigger()
    //{

    //}

}
